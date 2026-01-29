import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, UpdateDateColumn } from 'typeorm';

export enum IntegrationType {
  PAYMENT_GATEWAY = 'payment_gateway',
  EXTERNAL_API = 'external_api',
  DATABASE_SYNC = 'database_sync',
  FILE_SYNC = 'file_sync',
  EMAIL_SERVICE = 'email_service',
  SMS_SERVICE = 'sms_service',
  HEALTH_SYSTEM = 'health_system'
}

export enum IntegrationStatus {
  ACTIVE = 'active',
  INACTIVE = 'inactive',
  ERROR = 'error',
  MAINTENANCE = 'maintenance'
}

export enum PaymentProvider {
  STRIPE = 'stripe',
  PAYPAL = 'paypal',
  MERCADOPAGO = 'mercadopago',
  PAGSEGURO = 'pagseguro',
  CIELO = 'cielo'
}

export enum SyncDirection {
  INOVA_TO_EXTERNAL = 'inova_to_external',
  EXTERNAL_TO_INOVA = 'external_to_inova',
  BIDIRECTIONAL = 'bidirectional'
}

export enum SyncStatus {
  PENDING = 'pending',
  IN_PROGRESS = 'in_progress',
  COMPLETED = 'completed',
  FAILED = 'failed',
  PARTIAL = 'partial'
}

@Entity('integrations')
export class Integration {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text' })
  description: string;

  @Column({
    type: 'enum',
    enum: IntegrationType
  })
  type: IntegrationType;

  @Column({
    type: 'enum',
    enum: IntegrationStatus,
    default: IntegrationStatus.INACTIVE
  })
  status: IntegrationStatus;

  @Column({ type: 'jsonb' })
  configuration: any; // API keys, endpoints, credentials, etc.

  @Column({ type: 'jsonb', nullable: true })
  settings: any; // Additional settings

  @Column({ type: 'uuid' })
  createdBy: string;

  @Column({ type: 'uuid', nullable: true })
  updatedBy: string;

  @Column({ type: 'timestamp', nullable: true })
  lastSyncAt: Date;

  @Column({ type: 'text', nullable: true })
  lastSyncError: string;

  @Column({ type: 'int', default: 0 })
  syncCount: number;

  @Column({ type: 'int', default: 0 })
  errorCount: number;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}

@Entity('integration_logs')
export class IntegrationLog {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  integrationId: string;

  @Column({ type: 'varchar', length: 100 })
  operation: string; // 'sync', 'webhook', 'api_call', etc.

  @Column({
    type: 'enum',
    enum: SyncStatus,
    default: SyncStatus.PENDING
  })
  status: SyncStatus;

  @Column({ type: 'text', nullable: true })
  requestData: string;

  @Column({ type: 'text', nullable: true })
  responseData: string;

  @Column({ type: 'text', nullable: true })
  errorMessage: string;

  @Column({ type: 'int', nullable: true })
  recordsProcessed: number;

  @Column({ type: 'int', nullable: true })
  recordsFailed: number;

  @Column({ type: 'bigint', nullable: true })
  duration: number; // in milliseconds

  @Column({ type: 'jsonb', nullable: true })
  metadata: any;

  @CreateDateColumn()
  createdAt: Date;
}

@Entity('payment_transactions')
export class PaymentTransaction {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid', nullable: true })
  integrationId: string;

  @Column({
    type: 'enum',
    enum: PaymentProvider
  })
  provider: PaymentProvider;

  @Column({ type: 'varchar', length: 100 })
  transactionId: string; // External transaction ID

  @Column({ type: 'varchar', length: 50 })
  status: string; // 'pending', 'completed', 'failed', 'cancelled'

  @Column({ type: 'decimal', precision: 10, scale: 2 })
  amount: number;

  @Column({ type: 'varchar', length: 3, default: 'BRL' })
  currency: string;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({ type: 'uuid', nullable: true })
  relatedEntityId: string; // ID of related despesa, etc.

  @Column({ type: 'varchar', length: 50, nullable: true })
  relatedEntityType: string; // 'despesa', etc.

  @Column({ type: 'jsonb', nullable: true })
  paymentData: any; // Additional payment data

  @Column({ type: 'jsonb', nullable: true })
  metadata: any;

  @Column({ type: 'timestamp', nullable: true })
  processedAt: Date;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}

@Entity('external_syncs')
export class ExternalSync {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  integrationId: string;

  @Column({
    type: 'enum',
    enum: SyncDirection
  })
  direction: SyncDirection;

  @Column({ type: 'varchar', length: 100 })
  entityType: string; // 'ubs', 'despesa', 'user', etc.

  @Column({
    type: 'enum',
    enum: SyncStatus,
    default: SyncStatus.PENDING
  })
  status: SyncStatus;

  @Column({ type: 'int', default: 0 })
  recordsTotal: number;

  @Column({ type: 'int', default: 0 })
  recordsProcessed: number;

  @Column({ type: 'int', default: 0 })
  recordsFailed: number;

  @Column({ type: 'text', nullable: true })
  errorMessage: string;

  @Column({ type: 'jsonb', nullable: true })
  syncData: any;

  @Column({ type: 'uuid' })
  initiatedBy: string;

  @Column({ type: 'timestamp', nullable: true })
  completedAt: Date;

  @CreateDateColumn()
  createdAt: Date;
}

@Entity('api_endpoints')
export class ApiEndpoint {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'uuid' })
  integrationId: string;

  @Column({ type: 'varchar', length: 100 })
  name: string;

  @Column({ type: 'varchar', length: 10 })
  method: string; // GET, POST, PUT, DELETE

  @Column({ type: 'varchar', length: 500 })
  path: string;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({ type: 'jsonb', nullable: true })
  requestSchema: any; // JSON schema for request validation

  @Column({ type: 'jsonb', nullable: true })
  responseSchema: any; // JSON schema for response validation

  @Column({ type: 'jsonb', nullable: true })
  headers: any;

  @Column({ type: 'boolean', default: true })
  active: boolean;

  @Column({ type: 'int', default: 0 })
  callCount: number;

  @Column({ type: 'int', default: 0 })
  errorCount: number;

  @CreateDateColumn()
  createdAt: Date;

  @UpdateDateColumn()
  updatedAt: Date;
}