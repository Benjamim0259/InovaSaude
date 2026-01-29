import { Router } from 'express';
import { auditController } from './audit.controller';
import { AuthMiddleware } from '../../InovaSaude.Core/Middlewares/AuthMiddleware';

const router = Router();

// All audit routes require authentication
router.use(AuthMiddleware.authenticate);

// Audit Logs
router.get('/logs', auditController.getAuditLogs);
router.get('/users/:userId/activity', auditController.getUserActivity);
router.get('/entities/:entityType/:entityId/history', auditController.getEntityHistory);

// Entity Versions
router.get('/entities/:entityType/:entityId/versions', auditController.getEntityVersions);
router.get('/entities/:entityType/:entityId/versions/:version', auditController.getEntityVersion);
router.post('/entities/:entityType/:entityId/versions/:version/restore', auditController.restoreEntityVersion);

// System Events
router.get('/system-events', auditController.getSystemEvents);
router.post('/system-events/:eventId/acknowledge', auditController.acknowledgeSystemEvent);
router.post('/system-events', auditController.logSystemEvent);

// Data Exports
router.get('/data-exports', auditController.getDataExports);

// Statistics
router.get('/statistics', auditController.getAuditStatistics);

// Maintenance (admin only - you might want to add role checking)
router.post('/maintenance/cleanup-logs', auditController.cleanupOldLogs);

export default router;