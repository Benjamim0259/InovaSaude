import { Request, Response } from 'express';
import { WebhookEventType, WebhookStatus } from './webhook.entity';
import { webhookService } from './webhook.service';
import logger from '../../config/logger';

export class WebhookController {
  async createWebhook(req: Request, res: Response) {
    try {
      const { name, description, url, events, secret, retryCount, timeout, headers } = req.body;
      const createdBy = req.user?.id;

      if (!createdBy) {
        return res.status(401).json({ message: 'Usuário não autenticado' });
      }

      const webhook = await webhookService.createWebhook({
        name,
        description,
        url,
        events,
        secret,
        retryCount,
        timeout,
        headers,
        createdBy
      });

      logger.info(`Webhook criado: ${webhook.id} por usuário ${createdBy}`);
      res.status(201).json(webhook);
    } catch (error: any) {
      logger.error('Erro ao criar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWebhooks(req: Request, res: Response) {
    try {
      const webhooks = await webhookService.getWebhooks();
      res.json(webhooks);
    } catch (error: any) {
      logger.error('Erro ao buscar webhooks:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWebhookById(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const webhook = await webhookService.getWebhookById(id);

      if (!webhook) {
        return res.status(404).json({ message: 'Webhook não encontrado' });
      }

      res.json(webhook);
    } catch (error: any) {
      logger.error('Erro ao buscar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async updateWebhook(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const updateData = req.body;

      const webhook = await webhookService.updateWebhook(id, updateData);

      if (!webhook) {
        return res.status(404).json({ message: 'Webhook não encontrado' });
      }

      logger.info(`Webhook atualizado: ${id}`);
      res.json(webhook);
    } catch (error: any) {
      logger.error('Erro ao atualizar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async deleteWebhook(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const deleted = await webhookService.deleteWebhook(id);

      if (!deleted) {
        return res.status(404).json({ message: 'Webhook não encontrado' });
      }

      logger.info(`Webhook deletado: ${id}`);
      res.status(204).send();
    } catch (error: any) {
      logger.error('Erro ao deletar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWebhookLogs(req: Request, res: Response) {
    try {
      const { webhookId } = req.params;
      const { limit } = req.query;

      const logs = await webhookService.getWebhookLogs(
        webhookId,
        limit ? parseInt(limit as string) : 50
      );

      res.json(logs);
    } catch (error: any) {
      logger.error('Erro ao buscar logs de webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async reactivateWebhook(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const reactivated = await webhookService.reactivateWebhook(id);

      if (!reactivated) {
        return res.status(404).json({ message: 'Webhook não encontrado' });
      }

      logger.info(`Webhook reativado: ${id}`);
      res.json({ message: 'Webhook reativado com sucesso' });
    } catch (error: any) {
      logger.error('Erro ao reativar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async testWebhook(req: Request, res: Response) {
    try {
      const { id } = req.params;

      const webhook = await webhookService.getWebhookById(id);
      if (!webhook) {
        return res.status(404).json({ message: 'Webhook não encontrado' });
      }

      // Send a test payload
      const testPayload = {
        event: WebhookEventType.DESPESA_CREATED,
        data: {
          id: 'test-id',
          description: 'Test webhook payload',
          amount: 100.00,
          date: new Date().toISOString()
        },
        timestamp: new Date().toISOString(),
        source: 'inovasaude-test'
      };

      // This will trigger the actual webhook
      await webhookService.triggerEvent(
        WebhookEventType.DESPESA_CREATED,
        testPayload.data,
        'inovasaude-test'
      );

      res.json({ message: 'Webhook de teste enviado' });
    } catch (error: any) {
      logger.error('Erro ao testar webhook:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }
}

export const webhookController = new WebhookController();