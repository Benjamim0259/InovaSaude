import { Router } from 'express';
import { webhookController } from './webhook.controller';
import { AuthMiddleware } from '../../InovaSaude.Core/Middlewares/AuthMiddleware';

const router = Router();

// All webhook routes require authentication
router.use(AuthMiddleware.authenticate);

// Create webhook
router.post('/', webhookController.createWebhook);

// Get all webhooks
router.get('/', webhookController.getWebhooks);

// Get webhook by ID
router.get('/:id', webhookController.getWebhookById);

// Update webhook
router.put('/:id', webhookController.updateWebhook);

// Delete webhook
router.delete('/:id', webhookController.deleteWebhook);

// Get webhook logs
router.get('/:webhookId/logs', webhookController.getWebhookLogs);

// Reactivate webhook
router.post('/:id/reactivate', webhookController.reactivateWebhook);

// Test webhook
router.post('/:id/test', webhookController.testWebhook);

export default router;