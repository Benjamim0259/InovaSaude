import axios, { AxiosResponse } from 'axios';
import { PrismaClient, Webhook, WebhookLog, WebhookEventType, WebhookStatus } from '@prisma/client';
import logger from '../../config/logger';

const prisma = new PrismaClient();

export interface WebhookPayload {
  event: WebhookEventType;
  data: any;
  timestamp: string;
  source: string;
}

export class WebhookService {

  async createWebhook(data: {
    name: string;
    description: string;
    url: string;
    events: WebhookEventType[];
    secret?: string;
    retryCount?: number;
    timeout?: number;
    headers?: Record<string, string>;
    createdBy: string;
  }): Promise<Webhook> {
    return await prisma.webhook.create({
      data: {
        ...data,
        status: WebhookStatus.ACTIVE,
        retryCount: data.retryCount || 3,
        timeout: data.timeout || 5000,
        headers: data.headers ? JSON.stringify(data.headers) : null,
      }
    });
  }

  async updateWebhook(id: string, data: Partial<Webhook>): Promise<Webhook | null> {
    try {
      return await prisma.webhook.update({
        where: { id },
        data: {
          ...data,
          headers: data.headers ? JSON.stringify(data.headers) : undefined,
        }
      });
    } catch {
      return null;
    }
  }

  async deleteWebhook(id: string): Promise<boolean> {
    try {
      await prisma.webhook.delete({ where: { id } });
      return true;
    } catch {
      return false;
    }
  }

  async getWebhooks(): Promise<Webhook[]> {
    return await prisma.webhook.findMany({
      orderBy: { createdAt: 'desc' }
    });
  }

  async getWebhookById(id: string): Promise<Webhook | null> {
    return await prisma.webhook.findUnique({ where: { id } });
  }

  async triggerEvent(event: WebhookEventType, data: any, source: string = 'inovasaude'): Promise<void> {
    const webhooks = await prisma.webhook.findMany({
      where: {
        status: WebhookStatus.ACTIVE,
        events: {
          has: event
        }
      }
    });

    if (webhooks.length === 0) {
      logger.info(`No active webhooks found for event: ${event}`);
      return;
    }

    const payload: WebhookPayload = {
      event,
      data,
      timestamp: new Date().toISOString(),
      source
    };

    logger.info(`Triggering webhook event: ${event} for ${webhooks.length} webhooks`);

    for (const webhook of webhooks) {
      this.sendWebhook(webhook, payload);
    }
  }

  private async sendWebhook(webhook: Webhook, payload: WebhookPayload): Promise<void> {
    let success = false;
    let response = '';
    let error = '';
    let statusCode = 0;

    const parsedHeaders = webhook.headers ? JSON.parse(webhook.headers) : {};

    for (let attempt = 0; attempt <= webhook.retryCount; attempt++) {
      try {
        const headers: Record<string, string> = {
          'Content-Type': 'application/json',
          'User-Agent': 'InovaSaude-Webhook/1.0',
          ...parsedHeaders
        };

        if (webhook.secret) {
          const signature = this.generateSignature(JSON.stringify(payload), webhook.secret);
          headers['X-Webhook-Signature'] = signature;
        }

        const axiosResponse: AxiosResponse = await axios.post(webhook.url, payload, {
          headers,
          timeout: webhook.timeout,
          validateStatus: () => true // Don't throw on any status code
        });

        statusCode = axiosResponse.status;
        response = JSON.stringify(axiosResponse.data);

        if (axiosResponse.status >= 200 && axiosResponse.status < 300) {
          success = true;
          logger.info(`Webhook ${webhook.id} delivered successfully: ${webhook.url}`);
          break;
        } else {
          error = `HTTP ${axiosResponse.status}: ${response}`;
          logger.warn(`Webhook ${webhook.id} failed (attempt ${attempt + 1}): ${error}`);
        }

      } catch (err: any) {
        error = err.message;
        logger.error(`Webhook ${webhook.id} error (attempt ${attempt + 1}): ${error}`);
      }

      // Wait before retry (exponential backoff)
      if (attempt < webhook.retryCount) {
        await new Promise(resolve => setTimeout(resolve, Math.pow(2, attempt) * 1000));
      }
    }

    // Log the webhook attempt
    await prisma.webhookLog.create({
      data: {
        webhookId: webhook.id,
        event: payload.event,
        payload: JSON.stringify(payload),
        statusCode,
        response,
        error,
        retryCount: success ? 0 : webhook.retryCount,
        success
      }
    });

    // Deactivate webhook if it keeps failing
    if (!success) {
      const recentLogs = await prisma.webhookLog.findMany({
        where: { webhookId: webhook.id },
        orderBy: { createdAt: 'desc' },
        take: 5
      });

      const recentFailures = recentLogs.filter(log => !log.success).length;
      if (recentFailures >= 3) {
        await prisma.webhook.update({
          where: { id: webhook.id },
          data: { status: WebhookStatus.FAILED }
        });
        logger.warn(`Webhook ${webhook.id} deactivated due to repeated failures`);
      }
    }
  }

  private generateSignature(payload: string, secret: string): string {
    const crypto = require('crypto');
    return crypto.createHmac('sha256', secret).update(payload).digest('hex');
  }

  async getWebhookLogs(webhookId?: string, limit: number = 50): Promise<WebhookLog[]> {
    return await prisma.webhookLog.findMany({
      where: webhookId ? { webhookId } : {},
      orderBy: { createdAt: 'desc' },
      take: limit,
      include: { webhook: true }
    });
  }

  async reactivateWebhook(id: string): Promise<boolean> {
    try {
      await prisma.webhook.update({
        where: { id },
        data: { status: WebhookStatus.ACTIVE }
      });
      return true;
    } catch {
      return false;
    }
  }
}

export const webhookService = new WebhookService();