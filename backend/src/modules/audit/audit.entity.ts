import { Entity, PrimaryGeneratedColumn, Column, CreateDateColumn, Index } from 'typeorm';

export enum AuditAction {
  CREATE = 'CREATE',
  UPDATE = 'UPDATE',
  DELETE = 'DELETE',
  LOGIN = 'LOGIN',
  LOGOUT = 'LOGOUT',
  EXPORT = 'EXPORT',
  IMPORT = 'IMPORT',
  APPROVE = 'APPROVE',
  REJECT = 'REJECT',
  VIEW = 'VIEW',
  DOWNLOAD = 'DOWNLOAD'
}

export enum AuditEntityType {
  USER = 'user',
  UBS = 'ubs',
  DESPESA = 'despesa',
  WORKFLOW = 'workflow',
  WEBHOOK = 'webhook',
  PERMISSION = 'permission',
  SYSTEM = 'system'
}

export enum AuditSeverity {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  CRITICAL = 'critical'
}

@Entity('audit_logs')
@Index(['entityType', 'entityId'])
@Index(['userId', 'createdAt'])
@Index(['action', 'createdAt'])
@Index(['createdAt'])
export class AuditLog {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({
    type: 'enum',
    enum: AuditAction
  })
  action: AuditAction;

  @Column({
    type: 'enum',
    enum: AuditEntityType
  })
  entityType: AuditEntityType;

  @Column({ type: 'uuid', nullable: true })
  entityId: string;

  @Column({ type: 'uuid', nullable: true })
  userId: string;

  @Column({ type: 'varchar', length: 255, nullable: true })
  userEmail: string;

  @Column({ type: 'varchar', length: 255, nullable: true })
  userName: string;

  @Column({ type: 'jsonb', nullable: true })
  oldValues: any; // Previous state of the entity

  @Column({ type: 'jsonb', nullable: true })
  newValues: any; // New state of the entity

  @Column({ type: 'jsonb', nullable: true })
  changes: any; // Specific changes made

  @Column({ type: 'varchar', length: 45, nullable: true })
  ipAddress: string;

  @Column({ type: 'text', nullable: true })
  userAgent: string;

  @Column({ type: 'varchar', length: 10, nullable: true })
  sessionId: string;

  @Column({
    type: 'enum',
    enum: AuditSeverity,
    default: AuditSeverity.LOW
  })
  severity: AuditSeverity;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({ type: 'jsonb', nullable: true })
  metadata: any; // Additional context

  @CreateDateColumn()
  createdAt: Date;
}

@Entity('entity_versions')
@Index(['entityType', 'entityId', 'version'])
export class EntityVersion {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({
    type: 'enum',
    enum: AuditEntityType
  })
  entityType: AuditEntityType;

  @Column({ type: 'uuid' })
  entityId: string;

  @Column({ type: 'int' })
  version: number;

  @Column({ type: 'jsonb' })
  data: any; // Complete entity data at this version

  @Column({ type: 'uuid' })
  changedBy: string;

  @Column({ type: 'varchar', length: 255, nullable: true })
  changedByEmail: string;

  @Column({ type: 'varchar', length: 255, nullable: true })
  changeReason: string;

  @Column({ type: 'boolean', default: false })
  isActive: boolean; // Current active version

  @CreateDateColumn()
  createdAt: Date;
}

@Entity('system_events')
@Index(['eventType', 'createdAt'])
export class SystemEvent {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 100 })
  eventType: string;

  @Column({ type: 'varchar', length: 255 })
  title: string;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({
    type: 'enum',
    enum: AuditSeverity,
    default: AuditSeverity.LOW
  })
  severity: AuditSeverity;

  @Column({ type: 'jsonb', nullable: true })
  data: any;

  @Column({ type: 'varchar', length: 100, nullable: true })
  source: string; // Which part of the system generated this event

  @Column({ type: 'boolean', default: false })
  acknowledged: boolean;

  @Column({ type: 'uuid', nullable: true })
  acknowledgedBy: string;

  @Column({ type: 'timestamp', nullable: true })
  acknowledgedAt: Date;

  @CreateDateColumn()
  createdAt: Date;
}

@Entity('data_exports')
export class DataExport {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'varchar', length: 255 })
  name: string;

  @Column({ type: 'text', nullable: true })
  description: string;

  @Column({ type: 'varchar', length: 100 })
  exportType: string; // 'excel', 'pdf', 'csv', etc.

  @Column({ type: 'jsonb' })
  filters: any; // Applied filters

  @Column({ type: 'jsonb' })
  data: any; // Exported data or reference

  @Column({ type: 'varchar', length: 500, nullable: true })
  fileUrl: string;

  @Column({ type: 'bigint' })
  fileSize: number; // in bytes

  @Column({ type: 'uuid' })
  requestedBy: string;

  @Column({ type: 'varchar', length: 255, nullable: true })
  requestedByEmail: string;

  @Column({ type: 'timestamp', nullable: true })
  completedAt: Date;

  @Column({ type: 'int', nullable: true })
  recordCount: number;

  @CreateDateColumn()
  createdAt: Date;
}