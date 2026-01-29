import { Router } from 'express';
import { integrationController } from './integration.controller';
import { AuthMiddleware } from '../../InovaSaude.Core/Middlewares/AuthMiddleware';

const router = Router();

// All integration routes require authentication
router.use(AuthMiddleware.authenticate);

// Integration CRUD
router.post('/', integrationController.createIntegration);
router.get('/', integrationController.getIntegrations);
router.get('/:id', integrationController.getIntegrationById);
router.put('/:id', integrationController.updateIntegration);
router.delete('/:id', integrationController.deleteIntegration);

// Payment Processing
router.post('/:integrationId/payments', integrationController.processPayment);

// Data Synchronization
router.post('/:integrationId/sync', integrationController.syncData);

// External API Calls
router.post('/:integrationId/endpoints/:endpointId/call', integrationController.callExternalApi);

// API Endpoints Management
router.post('/endpoints', integrationController.createApiEndpoint);
router.get('/:integrationId/endpoints', integrationController.getApiEndpoints);

// Testing
router.post('/:id/test', integrationController.testIntegration);

// Logs and History
router.get('/:integrationId/logs', integrationController.getIntegrationLogs);
router.get('/:integrationId/sync-history', integrationController.getSyncHistory);
router.get('/:integrationId/payments', integrationController.getPaymentTransactions);

export default router;