import { PrismaClient, Workflow, WorkflowStep, WorkflowInstance, WorkflowStepInstance, WorkflowRule, WorkflowStatus, WorkflowStepStatus, ApprovalAction, WorkflowTriggerType } from '@prisma/client';
import { webhookService } from '../webhooks/webhook.service';
import { WebhookEventType } from '../webhooks/webhook.entity';
import logger from '../../config/logger';

const prisma = new PrismaClient();

export interface WorkflowTriggerData {
  entityType: string;
  entityId: string;
  event: string;
  data: any;
  userId: string;
}

export class WorkflowService {
  private workflowRepository: Repository<Workflow>;
  private stepRepository: Repository<WorkflowStep>;
  private instanceRepository: Repository<WorkflowInstance>;
  private stepInstanceRepository: Repository<WorkflowStepInstance>;
  private ruleRepository: Repository<WorkflowRule>;

  constructor() {
    this.workflowRepository = AppDataSource.getRepository(Workflow);
    this.stepRepository = AppDataSource.getRepository(WorkflowStep);
    this.instanceRepository = AppDataSource.getRepository(WorkflowInstance);
    this.stepInstanceRepository = AppDataSource.getRepository(WorkflowStepInstance);
    this.ruleRepository = AppDataSource.getRepository(WorkflowRule);
  }

  // Workflow CRUD
  async createWorkflow(data: {
    name: string;
    description: string;
    entityType: string;
    triggerType?: WorkflowTriggerType;
    triggerConditions?: any;
    steps: Array<{
      name: string;
      description?: string;
      type: string;
      configuration: any;
      conditions?: any;
      assignedTo?: string;
      timeoutHours?: number;
      required?: boolean;
    }>;
    createdBy: string;
  }): Promise<Workflow> {
    const workflow = this.workflowRepository.create({
      name: data.name,
      description: data.description,
      entityType: data.entityType,
      triggerType: data.triggerType || WorkflowTriggerType.MANUAL,
      triggerConditions: data.triggerConditions,
      status: WorkflowStatus.DRAFT,
      version: 1,
      createdBy: data.createdBy
    });

    const savedWorkflow = await this.workflowRepository.save(workflow);

    // Create steps
    for (let i = 0; i < data.steps.length; i++) {
      const step = this.stepRepository.create({
        workflowId: savedWorkflow.id,
        order: i + 1,
        name: data.steps[i].name,
        description: data.steps[i].description,
        type: data.steps[i].type as any,
        configuration: data.steps[i].configuration,
        conditions: data.steps[i].conditions,
        assignedTo: data.steps[i].assignedTo,
        timeoutHours: data.steps[i].timeoutHours,
        required: data.steps[i].required !== false
      });
      await this.stepRepository.save(step);
    }

    return await this.workflowRepository.findOne({
      where: { id: savedWorkflow.id },
      relations: ['steps']
    }) as Workflow;
  }

  async updateWorkflow(id: string, data: Partial<Workflow>): Promise<Workflow | null> {
    // If updating steps, handle separately
    if (data.steps) {
      await this.updateWorkflowSteps(id, data.steps);
      delete data.steps;
    }

    await this.workflowRepository.update(id, data);
    return await this.workflowRepository.findOne({
      where: { id },
      relations: ['steps']
    });
  }

  private async updateWorkflowSteps(workflowId: string, steps: WorkflowStep[]): Promise<void> {
    // Remove existing steps
    await this.stepRepository.delete({ workflowId });

    // Create new steps
    for (let i = 0; i < steps.length; i++) {
      const step = this.stepRepository.create({
        ...steps[i],
        workflowId,
        order: i + 1
      });
      await this.stepRepository.save(step);
    }
  }

  async deleteWorkflow(id: string): Promise<boolean> {
    const result = await this.workflowRepository.delete(id);
    return result.affected > 0;
  }

  async getWorkflows(entityType?: string): Promise<Workflow[]> {
    const where: any = {};
    if (entityType) {
      where.entityType = entityType;
    }

    return await this.workflowRepository.find({
      where,
      relations: ['steps'],
      order: { createdAt: 'DESC' }
    });
  }

  async getWorkflowById(id: string): Promise<Workflow | null> {
    return await this.workflowRepository.findOne({
      where: { id },
      relations: ['steps']
    });
  }

  async activateWorkflow(id: string): Promise<boolean> {
    const result = await this.workflowRepository.update(id, {
      status: WorkflowStatus.ACTIVE
    });
    return result.affected > 0;
  }

  async deactivateWorkflow(id: string): Promise<boolean> {
    const result = await this.workflowRepository.update(id, {
      status: WorkflowStatus.INACTIVE
    });
    return result.affected > 0;
  }

  // Workflow Rules
  async createWorkflowRule(data: {
    name: string;
    description: string;
    entityType: string;
    triggerEvent: string;
    conditions: any;
    workflowId: string;
    priority?: number;
    createdBy: string;
  }): Promise<WorkflowRule> {
    const rule = this.ruleRepository.create({
      ...data,
      priority: data.priority || 1,
      active: true
    });

    return await this.ruleRepository.save(rule);
  }

  async getWorkflowRules(entityType?: string): Promise<WorkflowRule[]> {
    const where: any = { active: true };
    if (entityType) {
      where.entityType = entityType;
    }

    return await this.ruleRepository.find({
      where,
      order: { priority: 'DESC', createdAt: 'DESC' }
    });
  }

  // Workflow Execution
  async triggerWorkflow(triggerData: WorkflowTriggerData): Promise<WorkflowInstance[]> {
    const { entityType, entityId, event, data, userId } = triggerData;

    // Find matching rules
    const rules = await this.ruleRepository.find({
      where: {
        entityType,
        triggerEvent: event,
        active: true
      },
      order: { priority: 'DESC' }
    });

    const instances: WorkflowInstance[] = [];

    for (const rule of rules) {
      // Check if conditions match
      if (this.evaluateConditions(rule.conditions, data)) {
        const workflow = await this.workflowRepository.findOne({
          where: { id: rule.workflowId, status: WorkflowStatus.ACTIVE },
          relations: ['steps']
        });

        if (workflow) {
          const instance = await this.startWorkflowInstance(workflow, entityId, userId, data);
          instances.push(instance);
        }
      }
    }

    return instances;
  }

  private async startWorkflowInstance(
    workflow: Workflow,
    entityId: string,
    userId: string,
    context: any
  ): Promise<WorkflowInstance> {
    const instance = this.instanceRepository.create({
      workflowId: workflow.id,
      entityType: workflow.entityType,
      entityId,
      status: WorkflowStepStatus.IN_PROGRESS,
      initiatedBy: userId,
      context
    });

    const savedInstance = await this.instanceRepository.save(instance);

    // Start first step
    if (workflow.steps && workflow.steps.length > 0) {
      await this.startWorkflowStep(savedInstance.id, workflow.steps[0]);
    }

    logger.info(`Workflow instance started: ${savedInstance.id} for workflow ${workflow.id}`);
    return savedInstance;
  }

  private async startWorkflowStep(instanceId: string, step: WorkflowStep): Promise<void> {
    const stepInstance = this.stepInstanceRepository.create({
      workflowInstanceId: instanceId,
      stepId: step.id,
      status: WorkflowStepStatus.IN_PROGRESS,
      assignedTo: step.assignedTo,
      startedAt: new Date(),
      dueDate: step.timeoutHours ? new Date(Date.now() + step.timeoutHours * 60 * 60 * 1000) : undefined
    });

    await this.stepInstanceRepository.save(stepInstance);

    // Trigger webhook for step started
    await webhookService.triggerEvent(
      WebhookEventType.WORKFLOW_COMPLETED,
      {
        instanceId,
        stepId: step.id,
        stepName: step.name,
        status: 'started'
      }
    );
  }

  async executeStepAction(
    stepInstanceId: string,
    action: ApprovalAction,
    userId: string,
    comments?: string,
    actionData?: any
  ): Promise<boolean> {
    const stepInstance = await this.stepInstanceRepository.findOne({
      where: { id: stepInstanceId },
      relations: ['workflowInstance', 'step']
    });

    if (!stepInstance || stepInstance.status !== WorkflowStepStatus.IN_PROGRESS) {
      return false;
    }

    // Update step instance
    stepInstance.status = WorkflowStepStatus.COMPLETED;
    stepInstance.completedBy = userId;
    stepInstance.action = action;
    stepInstance.comments = comments;
    stepInstance.actionData = actionData;
    stepInstance.completedAt = new Date();

    await this.stepInstanceRepository.save(stepInstance);

    // Get workflow instance
    const workflowInstance = await this.instanceRepository.findOne({
      where: { id: stepInstance.workflowInstanceId },
      relations: ['workflow', 'workflow.steps']
    });

    if (!workflowInstance) return false;

    // Determine next step or complete workflow
    const nextStep = await this.getNextStep(workflowInstance);
    if (nextStep) {
      await this.startWorkflowStep(workflowInstance.id, nextStep);
    } else {
      // Workflow completed
      workflowInstance.status = WorkflowStepStatus.COMPLETED;
      workflowInstance.completedAt = new Date();
      await this.instanceRepository.save(workflowInstance);

      // Trigger webhook
      await webhookService.triggerEvent(
        WebhookEventType.WORKFLOW_COMPLETED,
        {
          workflowId: workflowInstance.workflowId,
          instanceId: workflowInstance.id,
          entityType: workflowInstance.entityType,
          entityId: workflowInstance.entityId,
          finalAction: action
        }
      );

      logger.info(`Workflow instance completed: ${workflowInstance.id}`);
    }

    return true;
  }

  private async getNextStep(workflowInstance: WorkflowInstance): Promise<WorkflowStep | null> {
    const completedSteps = await this.stepInstanceRepository.find({
      where: { workflowInstanceId: workflowInstance.id }
    });

    const completedStepIds = completedSteps.map(s => s.stepId);
    const nextOrder = completedSteps.length + 1;

    return workflowInstance.workflow.steps.find(step => step.order === nextOrder) || null;
  }

  private evaluateConditions(conditions: any, data: any): boolean {
    if (!conditions) return true;

    // Simple condition evaluation - can be extended for complex rules
    for (const [key, value] of Object.entries(conditions)) {
      if (data[key] !== value) {
        return false;
      }
    }

    return true;
  }

  // Get workflow instances
  async getWorkflowInstances(entityType?: string, entityId?: string): Promise<WorkflowInstance[]> {
    const where: any = {};
    if (entityType) where.entityType = entityType;
    if (entityId) where.entityId = entityId;

    return await this.instanceRepository.find({
      where,
      relations: ['workflow', 'stepInstances'],
      order: { createdAt: 'DESC' }
    });
  }

  async getWorkflowInstanceById(id: string): Promise<WorkflowInstance | null> {
    return await this.instanceRepository.findOne({
      where: { id },
      relations: ['workflow', 'stepInstances', 'stepInstances.step']
    });
  }

  // Get pending steps for user
  async getPendingStepsForUser(userId: string): Promise<WorkflowStepInstance[]> {
    return await this.stepInstanceRepository.find({
      where: {
        assignedTo: userId,
        status: WorkflowStepStatus.IN_PROGRESS
      },
      relations: ['workflowInstance', 'step'],
      order: { createdAt: 'ASC' }
    });
  }
}

export const workflowService = new WorkflowService();