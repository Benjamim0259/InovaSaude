import { Request, Response } from 'express';
import { IntegrationType, PaymentProvider, SyncDirection } from './integration.entity';
import { integrationService, PaymentData, SyncOptions } from './integration.service';
import logger from '../../config/logger';

export class IntegrationController {
  // Integration CRUD
  async createIntegration(req: Request, res: Response) {
    try {
      const integration = await integrationService.createIntegration({
        ...req.body,
        createdBy: req.user?.id
      });

      logger.info(`Integração criada: ${integration.id}`);
      res.status(201).json(integration);
    } catch (error: any) {
      logger.error('Erro ao criar integração:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getIntegrations(req: Request, res: Response) {
    try {
      const { type } = req.query;
      const integrations = await integrationService.getIntegrations(type as IntegrationType);
      res.json(integrations);
    } catch (error: any) {
      logger.error('Erro ao buscar integrações:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getIntegrationById(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const integration = await integrationService.getIntegrationById(id);

      if (!integration) {
        return res.status(404).json({ message: 'Integração não encontrada' });
      }

      res.json(integration);
    } catch (error: any) {
      logger.error('Erro ao buscar integração:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async updateIntegration(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const integration = await integrationService.updateIntegration(id, {
        ...req.body,
        updatedBy: req.user?.id
      });

      if (!integration) {
        return res.status(404).json({ message: 'Integração não encontrada' });
      }

      logger.info(`Integração atualizada: ${id}`);
      res.json(integration);
    } catch (error: any) {
      logger.error('Erro ao atualizar integração:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async deleteIntegration(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const deleted = await integrationService.deleteIntegration(id);

      if (!deleted) {
        return res.status(404).json({ message: 'Integração não encontrada' });
      }

      logger.info(`Integração deletada: ${id}`);
      res.status(204).send();
    } catch (error: any) {
      logger.error('Erro ao deletar integração:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Payment Processing
  async processPayment(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const paymentData: PaymentData = req.body;

      const transaction = await integrationService.processPayment(integrationId, paymentData);

      logger.info(`Pagamento processado: ${transaction.id}`);
      res.json(transaction);
    } catch (error: any) {
      logger.error('Erro ao processar pagamento:', error);
      res.status(500).json({ message: error.message || 'Erro interno do servidor' });
    }
  }

  // Data Synchronization
  async syncData(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const syncOptions: SyncOptions = req.body;
      const initiatedBy = req.user?.id;

      if (!initiatedBy) {
        return res.status(401).json({ message: 'Usuário não autenticado' });
      }

      const sync = await integrationService.syncData(integrationId, syncOptions, initiatedBy);

      logger.info(`Sincronização iniciada: ${sync.id}`);
      res.json(sync);
    } catch (error: any) {
      logger.error('Erro ao sincronizar dados:', error);
      res.status(500).json({ message: error.message || 'Erro interno do servidor' });
    }
  }

  // External API Calls
  async callExternalApi(req: Request, res: Response) {
    try {
      const { integrationId, endpointId } = req.params;
      const data = req.body;

      const result = await integrationService.callExternalApi(integrationId, endpointId, data);

      res.json(result);
    } catch (error: any) {
      logger.error('Erro ao chamar API externa:', error);
      res.status(500).json({ message: error.message || 'Erro interno do servidor' });
    }
  }

  // API Endpoints Management
  async createApiEndpoint(req: Request, res: Response) {
    try {
      const endpoint = await integrationService.createApiEndpoint(req.body);

      logger.info(`Endpoint de API criado: ${endpoint.id}`);
      res.status(201).json(endpoint);
    } catch (error: any) {
      logger.error('Erro ao criar endpoint de API:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getApiEndpoints(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const endpoints = await integrationService.getApiEndpoints(integrationId);
      res.json(endpoints);
    } catch (error: any) {
      logger.error('Erro ao buscar endpoints de API:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Testing
  async testIntegration(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const result = await integrationService.testIntegration(id);
      res.json(result);
    } catch (error: any) {
      logger.error('Erro ao testar integração:', error);
      res.status(500).json({ message: error.message || 'Erro interno do servidor' });
    }
  }

  // Logs and History
  async getIntegrationLogs(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const { limit } = req.query;

      const logs = await integrationService.getIntegrationLogs(
        integrationId,
        limit ? parseInt(limit as string) : 50
      );

      res.json(logs);
    } catch (error: any) {
      logger.error('Erro ao buscar logs de integração:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getSyncHistory(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const { limit } = req.query;

      const history = await integrationService.getSyncHistory(
        integrationId,
        limit ? parseInt(limit as string) : 50
      );

      res.json(history);
    } catch (error: any) {
      logger.error('Erro ao buscar histórico de sincronização:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getPaymentTransactions(req: Request, res: Response) {
    try {
      const { integrationId } = req.params;
      const { limit } = req.query;

      const transactions = await integrationService.getPaymentTransactions(
        integrationId,
        limit ? parseInt(limit as string) : 50
      );

      res.json(transactions);
    } catch (error: any) {
      logger.error('Erro ao buscar transações de pagamento:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }
}

export const integrationController = new IntegrationController();