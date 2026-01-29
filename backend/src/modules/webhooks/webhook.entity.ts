import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, UpdateDateColumn } from 'typeorm';

export enum WebhookEventType {
  DESPESA_CREATED = 'despesa.created',
  DESPESA_UPDATED = 'despesa.updated',
  DESPESA_DELETED = 'despesa.deleted',
  DESPESA_APPROVED = 'despesa.approved',
  DESPESA_REJECTED = 'despesa.rejected',
  UBS_CREATED = 'ubs.created',
  UBS_UPDATED = 'ubs.updated',
  UBS_DELETED = 'ubs.deleted',
  USER_CREATED = 'user.created',
  USER_UPDATED = 'user.updated',
  WORKFLOW_COMPLETED = 'workflow.completed',
  PAYMENT_RECEIVED = 'payment.received',
  INTEGRATION_SYNC = 'integration.sync'
}

export enum WebhookStatus {
  ACTIVE = 'active',
  INACTIVE = 'inactive',
  FAILED = 'failed'
}

@Entity('webhooks')
export class Webhook {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text' })
  description: string;

  @Column({ type: 'varchar', length: 500 })
  url: string;

  @Column({
    type: 'enum',
    enum: WebhookEventType,
    array: true
  })
  events: WebhookEventType[];

  @Column({
    type: 'enum',
    enum: WebhookStatus,
    default: WebhookStatus.ACTIVE
  })
  status: WebhookStatus;

  @Column({ type: 'varchar', length: 255, nullable: true })
  secret: string;

  @Column({ type: 'int', default: 3 })
  retryCount: number;

  @Column({ type: 'int', default: 5000 })
  timeout: number;

  @Column({ type: 'jsonb', nullable: true })
  headers: Record<string, string>;

  @Column({ type: 'uuid' })
  createdBy: string;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}

@Entity('webhook_logs')
export class WebhookLog {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  webhookId: string;

  @Column({
    type: 'enum',
    enum: WebhookEventType
  })
  event: WebhookEventType;

  @Column({ type: 'jsonb' })
  payload: any;

  @Column({ type: 'int' })
  statusCode: number;

  @Column({ type: 'text', nullable: true })
  response: string;

  @Column({ type: 'text', nullable: true })
  error: string;

  @Column({ type: 'int', default: 0 })
  retryCount: number;

  @Column({ type: 'boolean', default: false })
  success: boolean;

  @CreateDateColumn()
  createdAt: Date;
}