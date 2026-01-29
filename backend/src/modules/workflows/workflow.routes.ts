import { Router } from 'express';
import { workflowController } from './workflow.controller';
import { AuthMiddleware } from '../../InovaSaude.Core/Middlewares/AuthMiddleware';

const router = Router();

// All workflow routes require authentication
router.use(AuthMiddleware.authenticate);

// Workflow CRUD
router.post('/', workflowController.createWorkflow);
router.get('/', workflowController.getWorkflows);
router.get('/:id', workflowController.getWorkflowById);
router.put('/:id', workflowController.updateWorkflow);
router.delete('/:id', workflowController.deleteWorkflow);

// Workflow status
router.post('/:id/activate', workflowController.activateWorkflow);
router.post('/:id/deactivate', workflowController.deactivateWorkflow);

// Workflow Rules
router.post('/rules', workflowController.createWorkflowRule);
router.get('/rules', workflowController.getWorkflowRules);

// Workflow Instances
router.get('/instances', workflowController.getWorkflowInstances);
router.get('/instances/:id', workflowController.getWorkflowInstanceById);

// Workflow Actions
router.post('/steps/:stepInstanceId/execute', workflowController.executeStepAction);
router.get('/steps/pending', workflowController.getPendingSteps);

// Manual trigger
router.post('/trigger', workflowController.triggerWorkflow);

export default router;