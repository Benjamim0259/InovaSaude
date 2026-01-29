import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, UpdateDateColumn, ManyToOne, OneToMany, JoinColumn } from 'typeorm';

export enum WorkflowStatus {
  DRAFT = 'draft',
  ACTIVE = 'active',
  INACTIVE = 'inactive'
}

export enum WorkflowTriggerType {
  MANUAL = 'manual',
  AUTOMATIC = 'automatic',
  SCHEDULED = 'scheduled'
}

export enum WorkflowStepType {
  APPROVAL = 'approval',
  REVIEW = 'review',
  NOTIFICATION = 'notification',
  ACTION = 'action',
  CONDITION = 'condition'
}

export enum WorkflowStepStatus {
  PENDING = 'pending',
  IN_PROGRESS = 'in_progress',
  COMPLETED = 'completed',
  FAILED = 'failed',
  SKIPPED = 'skipped'
}

export enum ApprovalAction {
  APPROVE = 'approve',
  REJECT = 'reject',
  RETURN = 'return',
  ESCALATE = 'escalate'
}

@Entity('workflows')
export class Workflow {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text' })
  description: string;

  @Column({
    type: 'enum',
    enum: WorkflowStatus,
    default: WorkflowStatus.DRAFT
  })
  status: WorkflowStatus;

  @Column({
    type: 'enum',
    enum: WorkflowTriggerType,
    default: WorkflowTriggerType.MANUAL
  })
  triggerType: WorkflowTriggerType;

  @Column({ type: 'varchar', length: 100 })
  entityType: string; // 'despesa', 'ubs', etc.

  @Column({ type: 'jsonb', nullable: true })
  triggerConditions: any; // Conditions that trigger the workflow

  @Column({ type: 'int', default: 1 })
  version: number;

  @Column({ type: 'uuid' })
  createdBy: string;

  @Column({ type: 'uuid', nullable: true })
  updatedBy: string;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;

  @OneToMany(() => WorkflowStep, step => step.workflow, { cascade: true })
  steps: WorkflowStep[];
}

@Entity('workflow_steps')
export class WorkflowStep {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  workflowId: string;

  @ManyToOne(() => Workflow, workflow => workflow.steps)
  @JoinColumn({ name: 'workflowId' })
  workflow: Workflow;

  @Column({ type: 'int' })
  order: number;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({
    type: 'enum',
    enum: WorkflowStepType
  })
  type: WorkflowStepType;

  @Column({ type: 'jsonb' })
  configuration: any; // Step-specific configuration

  @Column({ type: 'jsonb', nullable: true })
  conditions: any; // Conditions to execute this step

  @Column({ type: 'uuid', nullable: true })
  assignedTo: string; // User or role ID

  @Column({ type: 'int', nullable: true })
  timeoutHours: number; // Hours to complete the step

  @Column({ type: 'boolean', default: true })
  required: boolean; // Must this step be completed?

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}

@Entity('workflow_instances')
export class WorkflowInstance {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  workflowId: string;

  @ManyToOne(() => Workflow)
  @JoinColumn({ name: 'workflowId' })
  workflow: Workflow;

  @Column({ type: 'varchar', length: 100 })
  entityType: string;

  @Column({ type: 'uuid' })
  entityId: string; // ID of the entity being processed

  @Column({
    type: 'enum',
    enum: WorkflowStepStatus,
    default: WorkflowStepStatus.PENDING
  })
  status: WorkflowStepStatus;

  @Column({ type: 'uuid' })
  initiatedBy: string;

  @Column({ type: 'jsonb', nullable: true })
  context: any; // Additional context data

  @Column({ type: 'timestamp', nullable: true })
  completedAt: Date;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;

  @OneToMany(() => WorkflowStepInstance, step => step.workflowInstance, { cascade: true })
  stepInstances: WorkflowStepInstance[];
}

@Entity('workflow_step_instances')
export class WorkflowStepInstance {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  workflowInstanceId: string;

  @ManyToOne(() => WorkflowInstance, instance => instance.stepInstances)
  @JoinColumn({ name: 'workflowInstanceId' })
  workflowInstance: WorkflowInstance;

  @Column({ type: 'uuid' })
  stepId: string;

  @ManyToOne(() => WorkflowStep)
  @JoinColumn({ name: 'stepId' })
  step: WorkflowStep;

  @Column({
    type: 'enum',
    enum: WorkflowStepStatus,
    default: WorkflowStepStatus.PENDING
  })
  status: WorkflowStepStatus;

  @Column({ type: 'uuid', nullable: true })
  assignedTo: string;

  @Column({ type: 'uuid', nullable: true })
  completedBy: string;

  @Column({ type: 'text', nullable: true })
  comments: string;

  @Column({
    type: 'enum',
    enum: ApprovalAction,
    nullable: true
  })
  action: ApprovalAction;

  @Column({ type: 'jsonb', nullable: true })
  actionData: any; // Additional data from the action

  @Column({ type: 'timestamp', nullable: true })
  startedAt: Date;

  @Column({ type: 'timestamp', nullable: true })
  completedAt: Date;

  @Column({ type: 'timestamp', nullable: true })
  dueDate: Date;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}

@Entity('workflow_rules')
export class WorkflowRule {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text' })
  description: string;

  @Column({ type: 'varchar', length: 100 })
  entityType: string;

  @Column({ type: 'varchar', length: 100 })
  triggerEvent: string; // 'created', 'updated', 'deleted'

  @Column({ type: 'jsonb' })
  conditions: any; // Conditions to match

  @Column({ type: 'uuid' })
  workflowId: string;

  @Column({ type: 'boolean', default: true })
  active: boolean;

  @Column({ type: 'int', default: 1 })
  priority: number; // Higher priority rules are checked first

  @Column({ type: 'uuid' })
  createdBy: string;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}