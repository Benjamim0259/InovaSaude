import { Request, Response } from 'express';
import { ApprovalAction } from './workflow.entity';
import { workflowService, WorkflowTriggerData } from './workflow.service';
import logger from '../../config/logger';

export class WorkflowController {
  // Workflow CRUD
  async createWorkflow(req: Request, res: Response) {
    try {
      const workflow = await workflowService.createWorkflow({
        ...req.body,
        createdBy: req.user?.id
      });

      logger.info(`Workflow criado: ${workflow.id}`);
      res.status(201).json(workflow);
    } catch (error: any) {
      logger.error('Erro ao criar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWorkflows(req: Request, res: Response) {
    try {
      const { entityType } = req.query;
      const workflows = await workflowService.getWorkflows(entityType as string);
      res.json(workflows);
    } catch (error: any) {
      logger.error('Erro ao buscar workflows:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWorkflowById(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const workflow = await workflowService.getWorkflowById(id);

      if (!workflow) {
        return res.status(404).json({ message: 'Workflow não encontrado' });
      }

      res.json(workflow);
    } catch (error: any) {
      logger.error('Erro ao buscar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async updateWorkflow(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const workflow = await workflowService.updateWorkflow(id, req.body);

      if (!workflow) {
        return res.status(404).json({ message: 'Workflow não encontrado' });
      }

      logger.info(`Workflow atualizado: ${id}`);
      res.json(workflow);
    } catch (error: any) {
      logger.error('Erro ao atualizar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async deleteWorkflow(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const deleted = await workflowService.deleteWorkflow(id);

      if (!deleted) {
        return res.status(404).json({ message: 'Workflow não encontrado' });
      }

      logger.info(`Workflow deletado: ${id}`);
      res.status(204).send();
    } catch (error: any) {
      logger.error('Erro ao deletar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async activateWorkflow(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const activated = await workflowService.activateWorkflow(id);

      if (!activated) {
        return res.status(404).json({ message: 'Workflow não encontrado' });
      }

      logger.info(`Workflow ativado: ${id}`);
      res.json({ message: 'Workflow ativado com sucesso' });
    } catch (error: any) {
      logger.error('Erro ao ativar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async deactivateWorkflow(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const deactivated = await workflowService.deactivateWorkflow(id);

      if (!deactivated) {
        return res.status(404).json({ message: 'Workflow não encontrado' });
      }

      logger.info(`Workflow desativado: ${id}`);
      res.json({ message: 'Workflow desativado com sucesso' });
    } catch (error: any) {
      logger.error('Erro ao desativar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Workflow Rules
  async createWorkflowRule(req: Request, res: Response) {
    try {
      const rule = await workflowService.createWorkflowRule({
        ...req.body,
        createdBy: req.user?.id
      });

      logger.info(`Regra de workflow criada: ${rule.id}`);
      res.status(201).json(rule);
    } catch (error: any) {
      logger.error('Erro ao criar regra de workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWorkflowRules(req: Request, res: Response) {
    try {
      const { entityType } = req.query;
      const rules = await workflowService.getWorkflowRules(entityType as string);
      res.json(rules);
    } catch (error: any) {
      logger.error('Erro ao buscar regras de workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Workflow Instances
  async getWorkflowInstances(req: Request, res: Response) {
    try {
      const { entityType, entityId } = req.query;
      const instances = await workflowService.getWorkflowInstances(
        entityType as string,
        entityId as string
      );
      res.json(instances);
    } catch (error: any) {
      logger.error('Erro ao buscar instâncias de workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getWorkflowInstanceById(req: Request, res: Response) {
    try {
      const { id } = req.params;
      const instance = await workflowService.getWorkflowInstanceById(id);

      if (!instance) {
        return res.status(404).json({ message: 'Instância de workflow não encontrada' });
      }

      res.json(instance);
    } catch (error: any) {
      logger.error('Erro ao buscar instância de workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Execute workflow actions
  async executeStepAction(req: Request, res: Response) {
    try {
      const { stepInstanceId } = req.params;
      const { action, comments, actionData } = req.body;
      const userId = req.user?.id;

      if (!userId) {
        return res.status(401).json({ message: 'Usuário não autenticado' });
      }

      const success = await workflowService.executeStepAction(
        stepInstanceId,
        action,
        userId,
        comments,
        actionData
      );

      if (!success) {
        return res.status(400).json({ message: 'Não foi possível executar a ação' });
      }

      logger.info(`Ação executada no step ${stepInstanceId}: ${action}`);
      res.json({ message: 'Ação executada com sucesso' });
    } catch (error: any) {
      logger.error('Erro ao executar ação do workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Get pending steps for current user
  async getPendingSteps(req: Request, res: Response) {
    try {
      const userId = req.user?.id;

      if (!userId) {
        return res.status(401).json({ message: 'Usuário não autenticado' });
      }

      const steps = await workflowService.getPendingStepsForUser(userId);
      res.json(steps);
    } catch (error: any) {
      logger.error('Erro ao buscar steps pendentes:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Manual workflow trigger
  async triggerWorkflow(req: Request, res: Response) {
    try {
      const triggerData: WorkflowTriggerData = {
        ...req.body,
        userId: req.user?.id
      };

      const instances = await workflowService.triggerWorkflow(triggerData);

      logger.info(`Workflow manualmente disparado, ${instances.length} instâncias criadas`);
      res.json({
        message: `${instances.length} workflow(s) iniciado(s)`,
        instances: instances.map(i => i.id)
      });
    } catch (error: any) {
      logger.error('Erro ao disparar workflow:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }
}

export const workflowController = new WorkflowController();