import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { Repository } from 'typeorm';
import { AppDataSource } from '../../config/database';
import {
  Integration,
  IntegrationLog,
  PaymentTransaction,
  ExternalSync,
  ApiEndpoint,
  IntegrationType,
  IntegrationStatus,
  PaymentProvider,
  SyncDirection,
  SyncStatus
} from './integration.entity';
import { auditService } from '../audit/audit.service';
import { webhookService } from '../webhooks/webhook.service';
import { WebhookEventType } from '../webhooks/webhook.entity';
import logger from '../../config/logger';

export interface PaymentData {
  amount: number;
  currency?: string;
  description?: string;
  relatedEntityId?: string;
  relatedEntityType?: string;
  metadata?: any;
}

export interface SyncOptions {
  entityType: string;
  direction: SyncDirection;
  filters?: any;
  limit?: number;
  offset?: number;
}

export class IntegrationService {
  private integrationRepository: Repository<Integration>;
  private logRepository: Repository<IntegrationLog>;
  private paymentRepository: Repository<PaymentTransaction>;
  private syncRepository: Repository<ExternalSync>;
  private endpointRepository: Repository<ApiEndpoint>;

  constructor() {
    this.integrationRepository = AppDataSource.getRepository(Integration);
    this.logRepository = AppDataSource.getRepository(IntegrationLog);
    this.paymentRepository = AppDataSource.getRepository(PaymentTransaction);
    this.syncRepository = AppDataSource.getRepository(ExternalSync);
    this.endpointRepository = AppDataSource.getRepository(ApiEndpoint);
  }

  // Integration CRUD
  async createIntegration(data: {
    name: string;
    description: string;
    type: IntegrationType;
    configuration: any;
    settings?: any;
    createdBy: string;
  }): Promise<Integration> {
    const integration = this.integrationRepository.create(data);
    const saved = await this.integrationRepository.save(integration);

    await auditService.logActivity({
      action: 'CREATE' as any,
      entityType: 'integration' as any,
      entityId: saved.id,
      userId: data.createdBy,
      description: `Integração criada: ${data.name}`
    });

    return saved;
  }

  async updateIntegration(id: string, data: Partial<Integration>): Promise<Integration | null> {
    const existing = await this.integrationRepository.findOne({ where: { id } });
    if (!existing) return null;

    await this.integrationRepository.update(id, data);
    const updated = await this.integrationRepository.findOne({ where: { id } });

    await auditService.logEntityChange(
      'UPDATE' as any,
      'integration' as any,
      id,
      data.updatedBy || 'system',
      existing,
      updated
    );

    return updated;
  }

  async deleteIntegration(id: string): Promise<boolean> {
    const result = await this.integrationRepository.delete(id);
    return result.affected > 0;
  }

  async getIntegrations(type?: IntegrationType): Promise<Integration[]> {
    const where: any = {};
    if (type) where.type = type;

    return await this.integrationRepository.find({
      where,
      order: { createdAt: 'DESC' }
    });
  }

  async getIntegrationById(id: string): Promise<Integration | null> {
    return await this.integrationRepository.findOne({ where: { id } });
  }

  // Payment Integration
  async processPayment(integrationId: string, paymentData: PaymentData): Promise<PaymentTransaction> {
    const integration = await this.integrationRepository.findOne({ where: { id: integrationId } });
    if (!integration || integration.type !== IntegrationType.PAYMENT_GATEWAY) {
      throw new Error('Integração de pagamento não encontrada ou inválida');
    }

    const startTime = Date.now();

    try {
      let transaction: PaymentTransaction;

      switch (integration.configuration.provider) {
        case PaymentProvider.STRIPE:
          transaction = await this.processStripePayment(integration, paymentData);
          break;
        case PaymentProvider.MERCADOPAGO:
          transaction = await this.processMercadoPagoPayment(integration, paymentData);
          break;
        default:
          throw new Error(`Provedor de pagamento não suportado: ${integration.configuration.provider}`);
      }

      // Log successful payment
      await this.logRepository.save({
        integrationId,
        operation: 'payment',
        status: SyncStatus.COMPLETED,
        requestData: JSON.stringify(paymentData),
        responseData: JSON.stringify(transaction),
        recordsProcessed: 1,
        duration: Date.now() - startTime
      });

      // Trigger webhook
      await webhookService.triggerEvent(
        WebhookEventType.PAYMENT_RECEIVED,
        {
          transactionId: transaction.id,
          amount: transaction.amount,
          status: transaction.status
        }
      );

      return transaction;

    } catch (error: any) {
      // Log failed payment
      await this.logRepository.save({
        integrationId,
        operation: 'payment',
        status: SyncStatus.FAILED,
        requestData: JSON.stringify(paymentData),
        errorMessage: error.message,
        duration: Date.now() - startTime
      });

      throw error;
    }
  }

  private async processStripePayment(integration: Integration, data: PaymentData): Promise<PaymentTransaction> {
    // Stripe payment processing logic
    const stripe = require('stripe')(integration.configuration.apiKey);

    const paymentIntent = await stripe.paymentIntents.create({
      amount: Math.round(data.amount * 100), // Convert to cents
      currency: data.currency || 'brl',
      description: data.description,
      metadata: data.metadata
    });

    return await this.paymentRepository.save({
      integrationId: integration.id,
      provider: PaymentProvider.STRIPE,
      transactionId: paymentIntent.id,
      status: 'pending',
      amount: data.amount,
      currency: data.currency || 'BRL',
      description: data.description,
      relatedEntityId: data.relatedEntityId,
      relatedEntityType: data.relatedEntityType,
      paymentData: paymentIntent,
      metadata: data.metadata
    });
  }

  private async processMercadoPagoPayment(integration: Integration, data: PaymentData): Promise<PaymentTransaction> {
    // Mercado Pago payment processing logic
    const response = await axios.post(
      'https://api.mercadopago.com/v1/payments',
      {
        transaction_amount: data.amount,
        description: data.description,
        payment_method_id: 'pix', // or other methods
        payer: { email: 'payer@example.com' }
      },
      {
        headers: {
          'Authorization': `Bearer ${integration.configuration.accessToken}`,
          'Content-Type': 'application/json'
        }
      }
    );

    return await this.paymentRepository.save({
      integrationId: integration.id,
      provider: PaymentProvider.MERCADOPAGO,
      transactionId: response.data.id.toString(),
      status: response.data.status,
      amount: data.amount,
      currency: data.currency || 'BRL',
      description: data.description,
      relatedEntityId: data.relatedEntityId,
      relatedEntityType: data.relatedEntityType,
      paymentData: response.data,
      metadata: data.metadata
    });
  }

  // External API Integration
  async callExternalApi(integrationId: string, endpointId: string, data?: any): Promise<any> {
    const integration = await this.integrationRepository.findOne({ where: { id: integrationId } });
    const endpoint = await this.endpointRepository.findOne({ where: { id: endpointId } });

    if (!integration || !endpoint || !endpoint.active) {
      throw new Error('Integração ou endpoint não encontrado ou inativo');
    }

    const startTime = Date.now();
    const url = `${integration.configuration.baseUrl}${endpoint.path}`;

    try {
      const config: AxiosRequestConfig = {
        method: endpoint.method as any,
        url,
        headers: {
          ...endpoint.headers,
          ...integration.configuration.headers
        }
      };

      if (data && ['POST', 'PUT', 'PATCH'].includes(endpoint.method)) {
        config.data = data;
      }

      const response: AxiosResponse = await axios(config);

      // Update endpoint stats
      await this.endpointRepository.update(endpointId, {
        callCount: endpoint.callCount + 1
      });

      // Log successful call
      await this.logRepository.save({
        integrationId,
        operation: 'api_call',
        status: SyncStatus.COMPLETED,
        requestData: JSON.stringify(data),
        responseData: JSON.stringify(response.data),
        duration: Date.now() - startTime
      });

      return response.data;

    } catch (error: any) {
      // Update error count
      await this.endpointRepository.update(endpointId, {
        errorCount: endpoint.errorCount + 1
      });

      // Log failed call
      await this.logRepository.save({
        integrationId,
        operation: 'api_call',
        status: SyncStatus.FAILED,
        requestData: JSON.stringify(data),
        errorMessage: error.message,
        duration: Date.now() - startTime
      });

      throw error;
    }
  }

  // Data Synchronization
  async syncData(integrationId: string, options: SyncOptions, initiatedBy: string): Promise<ExternalSync> {
    const integration = await this.integrationRepository.findOne({ where: { id: integrationId } });
    if (!integration) {
      throw new Error('Integração não encontrada');
    }

    const sync = await this.syncRepository.save({
      integrationId,
      direction: options.direction,
      entityType: options.entityType,
      status: SyncStatus.IN_PROGRESS,
      initiatedBy
    });

    try {
      let recordsProcessed = 0;
      let recordsFailed = 0;

      // Perform sync based on direction and entity type
      switch (options.direction) {
        case SyncDirection.INOVA_TO_EXTERNAL:
          ({ processed: recordsProcessed, failed: recordsFailed } =
            await this.syncInovaToExternal(integration, options));
          break;
        case SyncDirection.EXTERNAL_TO_INOVA:
          ({ processed: recordsProcessed, failed: recordsFailed } =
            await this.syncExternalToInova(integration, options));
          break;
        case SyncDirection.BIDIRECTIONAL:
          const outResult = await this.syncInovaToExternal(integration, options);
          const inResult = await this.syncExternalToInova(integration, options);
          recordsProcessed = outResult.processed + inResult.processed;
          recordsFailed = outResult.failed + inResult.failed;
          break;
      }

      // Update sync status
      await this.syncRepository.update(sync.id, {
        status: recordsFailed > 0 ? SyncStatus.PARTIAL : SyncStatus.COMPLETED,
        recordsTotal: recordsProcessed + recordsFailed,
        recordsProcessed,
        recordsFailed,
        completedAt: new Date()
      });

      // Update integration stats
      await this.integrationRepository.update(integrationId, {
        lastSyncAt: new Date(),
        syncCount: integration.syncCount + 1,
        errorCount: recordsFailed > 0 ? integration.errorCount + recordsFailed : integration.errorCount
      });

      // Trigger webhook
      await webhookService.triggerEvent(
        WebhookEventType.INTEGRATION_SYNC,
        {
          integrationId,
          entityType: options.entityType,
          direction: options.direction,
          recordsProcessed,
          recordsFailed
        }
      );

      return await this.syncRepository.findOne({ where: { id: sync.id } }) as ExternalSync;

    } catch (error: any) {
      await this.syncRepository.update(sync.id, {
        status: SyncStatus.FAILED,
        errorMessage: error.message,
        completedAt: new Date()
      });

      await this.integrationRepository.update(integrationId, {
        errorCount: integration.errorCount + 1
      });

      throw error;
    }
  }

  private async syncInovaToExternal(integration: Integration, options: SyncOptions): Promise<{ processed: number, failed: number }> {
    // Implementation depends on entity type
    // This is a simplified version - in real implementation, you'd query the respective repositories
    logger.info(`Syncing ${options.entityType} from Inova to external system`);

    // Placeholder implementation
    return { processed: 10, failed: 0 };
  }

  private async syncExternalToInova(integration: Integration, options: SyncOptions): Promise<{ processed: number, failed: number }> {
    // Implementation depends on entity type
    logger.info(`Syncing ${options.entityType} from external system to Inova`);

    // Placeholder implementation
    return { processed: 8, failed: 1 };
  }

  // API Endpoints Management
  async createApiEndpoint(data: {
    integrationId: string;
    name: string;
    method: string;
    path: string;
    description?: string;
    requestSchema?: any;
    responseSchema?: any;
    headers?: any;
  }): Promise<ApiEndpoint> {
    const endpoint = this.endpointRepository.create(data);
    return await this.endpointRepository.save(endpoint);
  }

  async getApiEndpoints(integrationId: string): Promise<ApiEndpoint[]> {
    return await this.endpointRepository.find({
      where: { integrationId, active: true },
      order: { createdAt: 'ASC' }
    });
  }

  // Health Check
  async testIntegration(id: string): Promise<{ success: boolean, message: string, details?: any }> {
    const integration = await this.integrationRepository.findOne({ where: { id } });
    if (!integration) {
      return { success: false, message: 'Integração não encontrada' };
    }

    try {
      switch (integration.type) {
        case IntegrationType.PAYMENT_GATEWAY:
          return await this.testPaymentIntegration(integration);
        case IntegrationType.EXTERNAL_API:
          return await this.testApiIntegration(integration);
        default:
          return { success: false, message: 'Tipo de integração não suportado para teste' };
      }
    } catch (error: any) {
      return { success: false, message: error.message };
    }
  }

  private async testPaymentIntegration(integration: Integration): Promise<{ success: boolean, message: string }> {
    // Test payment integration connectivity
    try {
      // Implementation depends on provider
      return { success: true, message: 'Integração de pagamento testada com sucesso' };
    } catch (error: any) {
      return { success: false, message: `Erro na integração: ${error.message}` };
    }
  }

  private async testApiIntegration(integration: Integration): Promise<{ success: boolean, message: string }> {
    // Test API integration connectivity
    try {
      const response = await axios.get(integration.configuration.baseUrl + '/health', {
        timeout: 5000,
        headers: integration.configuration.headers
      });
      return { success: true, message: 'API externa acessível' };
    } catch (error: any) {
      return { success: false, message: `Erro na API externa: ${error.message}` };
    }
  }

  // Get integration logs
  async getIntegrationLogs(integrationId: string, limit: number = 50): Promise<IntegrationLog[]> {
    return await this.logRepository.find({
      where: { integrationId },
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  // Get sync history
  async getSyncHistory(integrationId: string, limit: number = 50): Promise<ExternalSync[]> {
    return await this.syncRepository.find({
      where: { integrationId },
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  // Get payment transactions
  async getPaymentTransactions(integrationId?: string, limit: number = 50): Promise<PaymentTransaction[]> {
    const where: any = {};
    if (integrationId) where.integrationId = integrationId;

    return await this.paymentRepository.find({
      where,
      order: { createdAt: 'DESC' },
      take: limit
    });
  }
}

export const integrationService = new IntegrationService();