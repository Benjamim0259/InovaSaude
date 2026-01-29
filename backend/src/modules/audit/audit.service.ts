import { Repository } from 'typeorm';
import { AppDataSource } from '../../config/database';
import { AuditLog, EntityVersion, SystemEvent, DataExport, AuditAction, AuditEntityType, AuditSeverity } from './audit.entity';
import logger from '../../config/logger';

export interface AuditLogData {
  action: AuditAction;
  entityType: AuditEntityType;
  entityId?: string;
  userId?: string;
  userEmail?: string;
  userName?: string;
  oldValues?: any;
  newValues?: any;
  changes?: any;
  ipAddress?: string;
  userAgent?: string;
  sessionId?: string;
  severity?: AuditSeverity;
  description?: string;
  metadata?: any;
}

export interface VersionData {
  entityType: AuditEntityType;
  entityId: string;
  data: any;
  changedBy: string;
  changedByEmail?: string;
  changeReason?: string;
}

export class AuditService {
  private auditLogRepository: Repository<AuditLog>;
  private versionRepository: Repository<EntityVersion>;
  private systemEventRepository: Repository<SystemEvent>;
  private dataExportRepository: Repository<DataExport>;

  constructor() {
    this.auditLogRepository = AppDataSource.getRepository(AuditLog);
    this.versionRepository = AppDataSource.getRepository(EntityVersion);
    this.systemEventRepository = AppDataSource.getRepository(SystemEvent);
    this.dataExportRepository = AppDataSource.getRepository(DataExport);
  }

  // Audit Logging
  async logActivity(logData: AuditLogData): Promise<void> {
    try {
      const log = this.auditLogRepository.create({
        ...logData,
        severity: logData.severity || AuditSeverity.LOW
      });

      await this.auditLogRepository.save(log);

      // Log critical events
      if (logData.severity === AuditSeverity.CRITICAL) {
        logger.error(`CRITICAL AUDIT EVENT: ${logData.action} on ${logData.entityType}`, {
          entityId: logData.entityId,
          userId: logData.userId,
          description: logData.description
        });
      }
    } catch (error) {
      logger.error('Failed to log audit activity:', error);
      // Don't throw - audit logging should not break business logic
    }
  }

  async logEntityChange(
    action: AuditAction,
    entityType: AuditEntityType,
    entityId: string,
    userId: string,
    oldValues?: any,
    newValues?: any,
    metadata?: any
  ): Promise<void> {
    const changes = this.calculateChanges(oldValues, newValues);

    await this.logActivity({
      action,
      entityType,
      entityId,
      userId,
      oldValues,
      newValues,
      changes,
      severity: this.determineSeverity(action, changes),
      description: `${action} ${entityType} ${entityId}`,
      metadata
    });
  }

  private calculateChanges(oldValues: any, newValues: any): any {
    if (!oldValues || !newValues) return null;

    const changes: any = {};

    // Get all keys from both objects
    const allKeys = new Set([...Object.keys(oldValues), ...Object.keys(newValues)]);

    for (const key of allKeys) {
      const oldValue = oldValues[key];
      const newValue = newValues[key];

      // Deep comparison for objects/arrays
      if (JSON.stringify(oldValue) !== JSON.stringify(newValue)) {
        changes[key] = {
          from: oldValue,
          to: newValue
        };
      }
    }

    return Object.keys(changes).length > 0 ? changes : null;
  }

  private determineSeverity(action: AuditAction, changes: any): AuditSeverity {
    if (action === AuditAction.DELETE) return AuditSeverity.HIGH;
    if (action === AuditAction.APPROVE || action === AuditAction.REJECT) return AuditSeverity.MEDIUM;
    if (changes && Object.keys(changes).length > 5) return AuditSeverity.MEDIUM;
    return AuditSeverity.LOW;
  }

  // Entity Versioning
  async createEntityVersion(versionData: VersionData): Promise<EntityVersion> {
    // Deactivate previous version
    await this.versionRepository.update(
      { entityType: versionData.entityType, entityId: versionData.entityId, isActive: true },
      { isActive: false }
    );

    // Get next version number
    const lastVersion = await this.versionRepository.findOne({
      where: { entityType: versionData.entityType, entityId: versionData.entityId },
      order: { version: 'DESC' }
    });

    const nextVersion = (lastVersion?.version || 0) + 1;

    const version = this.versionRepository.create({
      ...versionData,
      version: nextVersion,
      isActive: true
    });

    return await this.versionRepository.save(version);
  }

  async getEntityVersions(entityType: AuditEntityType, entityId: string): Promise<EntityVersion[]> {
    return await this.versionRepository.find({
      where: { entityType, entityId },
      order: { version: 'DESC' }
    });
  }

  async getEntityVersion(entityType: AuditEntityType, entityId: string, version: number): Promise<EntityVersion | null> {
    return await this.versionRepository.findOne({
      where: { entityType, entityId, version }
    });
  }

  async restoreEntityVersion(entityType: AuditEntityType, entityId: string, version: number, restoredBy: string): Promise<EntityVersion | null> {
    const targetVersion = await this.getEntityVersion(entityType, entityId, version);
    if (!targetVersion) return null;

    // Create new version with restored data
    await this.createEntityVersion({
      entityType,
      entityId,
      data: targetVersion.data,
      changedBy: restoredBy,
      changeReason: `Restored to version ${version}`
    });

    return targetVersion;
  }

  // System Events
  async logSystemEvent(
    eventType: string,
    title: string,
    description?: string,
    severity: AuditSeverity = AuditSeverity.LOW,
    data?: any,
    source?: string
  ): Promise<SystemEvent> {
    const event = this.systemEventRepository.create({
      eventType,
      title,
      description,
      severity,
      data,
      source
    });

    const savedEvent = await this.systemEventRepository.save(event);

    logger.info(`System event logged: ${eventType} - ${title}`, { severity, source });

    return savedEvent;
  }

  async acknowledgeSystemEvent(eventId: string, userId: string): Promise<boolean> {
    const result = await this.systemEventRepository.update(eventId, {
      acknowledged: true,
      acknowledgedBy: userId,
      acknowledgedAt: new Date()
    });

    return result.affected > 0;
  }

  async getSystemEvents(acknowledged?: boolean, limit: number = 50): Promise<SystemEvent[]> {
    const where: any = {};
    if (acknowledged !== undefined) {
      where.acknowledged = acknowledged;
    }

    return await this.systemEventRepository.find({
      where,
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  // Data Exports
  async logDataExport(data: {
    name: string;
    description?: string;
    exportType: string;
    filters: any;
    data: any;
    fileUrl?: string;
    fileSize: number;
    requestedBy: string;
    requestedByEmail?: string;
    recordCount?: number;
  }): Promise<DataExport> {
    const exportLog = this.dataExportRepository.create({
      ...data,
      completedAt: new Date()
    });

    return await this.dataExportRepository.save(exportLog);
  }

  async getDataExports(requestedBy?: string, limit: number = 50): Promise<DataExport[]> {
    const where: any = {};
    if (requestedBy) {
      where.requestedBy = requestedBy;
    }

    return await this.dataExportRepository.find({
      where,
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  // Query Methods
  async getAuditLogs(
    entityType?: AuditEntityType,
    entityId?: string,
    userId?: string,
    action?: AuditAction,
    startDate?: Date,
    endDate?: Date,
    limit: number = 100
  ): Promise<AuditLog[]> {
    const where: any = {};

    if (entityType) where.entityType = entityType;
    if (entityId) where.entityId = entityId;
    if (userId) where.userId = userId;
    if (action) where.action = action;

    if (startDate || endDate) {
      where.createdAt = {};
      if (startDate) where.createdAt.$gte = startDate;
      if (endDate) where.createdAt.$lte = endDate;
    }

    return await this.auditLogRepository.find({
      where,
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  async getUserActivity(userId: string, limit: number = 50): Promise<AuditLog[]> {
    return await this.auditLogRepository.find({
      where: { userId },
      order: { createdAt: 'DESC' },
      take: limit
    });
  }

  async getEntityHistory(entityType: AuditEntityType, entityId: string): Promise<AuditLog[]> {
    return await this.auditLogRepository.find({
      where: { entityType, entityId },
      order: { createdAt: 'DESC' }
    });
  }

  // Statistics
  async getAuditStatistics(startDate: Date, endDate: Date): Promise<any> {
    const logs = await this.auditLogRepository
      .createQueryBuilder('log')
      .select([
        'COUNT(*) as totalLogs',
        'COUNT(CASE WHEN severity = :high THEN 1 END) as highSeverityLogs',
        'COUNT(CASE WHEN severity = :critical THEN 1 END) as criticalLogs',
        'COUNT(DISTINCT userId) as uniqueUsers',
        'COUNT(DISTINCT CONCAT(entityType, entityId)) as uniqueEntities'
      ])
      .setParameters({ high: AuditSeverity.HIGH, critical: AuditSeverity.CRITICAL })
      .where('createdAt BETWEEN :startDate AND :endDate', { startDate, endDate })
      .getRawOne();

    return logs;
  }

  // Cleanup old logs (for maintenance)
  async cleanupOldLogs(daysToKeep: number = 365): Promise<number> {
    const cutoffDate = new Date();
    cutoffDate.setDate(cutoffDate.getDate() - daysToKeep);

    const result = await this.auditLogRepository.delete({
      createdAt: { $lt: cutoffDate } as any,
      severity: AuditSeverity.LOW
    });

    logger.info(`Cleaned up ${result.affected} old audit logs`);
    return result.affected || 0;
  }
}

export const auditService = new AuditService();