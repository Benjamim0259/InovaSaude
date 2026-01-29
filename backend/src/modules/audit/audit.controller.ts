import { Request, Response } from 'express';
import { AuditEntityType, AuditAction, AuditSeverity } from './audit.entity';
import { auditService } from './audit.service';
import logger from '../../config/logger';

export class AuditController {
  // Audit Logs
  async getAuditLogs(req: Request, res: Response) {
    try {
      const {
        entityType,
        entityId,
        userId,
        action,
        startDate,
        endDate,
        limit
      } = req.query;

      const logs = await auditService.getAuditLogs(
        entityType as AuditEntityType,
        entityId as string,
        userId as string,
        action as AuditAction,
        startDate ? new Date(startDate as string) : undefined,
        endDate ? new Date(endDate as string) : undefined,
        limit ? parseInt(limit as string) : 100
      );

      res.json(logs);
    } catch (error: any) {
      logger.error('Erro ao buscar logs de auditoria:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getUserActivity(req: Request, res: Response) {
    try {
      const { userId } = req.params;
      const { limit } = req.query;

      const logs = await auditService.getUserActivity(
        userId,
        limit ? parseInt(limit as string) : 50
      );

      res.json(logs);
    } catch (error: any) {
      logger.error('Erro ao buscar atividade do usuário:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getEntityHistory(req: Request, res: Response) {
    try {
      const { entityType, entityId } = req.params;

      const history = await auditService.getEntityHistory(
        entityType as AuditEntityType,
        entityId
      );

      res.json(history);
    } catch (error: any) {
      logger.error('Erro ao buscar histórico da entidade:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Entity Versions
  async getEntityVersions(req: Request, res: Response) {
    try {
      const { entityType, entityId } = req.params;

      const versions = await auditService.getEntityVersions(
        entityType as AuditEntityType,
        entityId
      );

      res.json(versions);
    } catch (error: any) {
      logger.error('Erro ao buscar versões da entidade:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async getEntityVersion(req: Request, res: Response) {
    try {
      const { entityType, entityId, version } = req.params;

      const entityVersion = await auditService.getEntityVersion(
        entityType as AuditEntityType,
        entityId,
        parseInt(version)
      );

      if (!entityVersion) {
        return res.status(404).json({ message: 'Versão não encontrada' });
      }

      res.json(entityVersion);
    } catch (error: any) {
      logger.error('Erro ao buscar versão da entidade:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async restoreEntityVersion(req: Request, res: Response) {
    try {
      const { entityType, entityId, version } = req.params;
      const { restoredBy } = req.body;

      const restoredVersion = await auditService.restoreEntityVersion(
        entityType as AuditEntityType,
        entityId,
        parseInt(version),
        restoredBy
      );

      if (!restoredVersion) {
        return res.status(404).json({ message: 'Versão não encontrada' });
      }

      logger.info(`Entidade restaurada: ${entityType} ${entityId} para versão ${version}`);
      res.json({
        message: 'Entidade restaurada com sucesso',
        version: restoredVersion
      });
    } catch (error: any) {
      logger.error('Erro ao restaurar versão da entidade:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // System Events
  async getSystemEvents(req: Request, res: Response) {
    try {
      const { acknowledged, limit } = req.query;

      const events = await auditService.getSystemEvents(
        acknowledged ? acknowledged === 'true' : undefined,
        limit ? parseInt(limit as string) : 50
      );

      res.json(events);
    } catch (error: any) {
      logger.error('Erro ao buscar eventos do sistema:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async acknowledgeSystemEvent(req: Request, res: Response) {
    try {
      const { eventId } = req.params;
      const userId = req.user?.id;

      if (!userId) {
        return res.status(401).json({ message: 'Usuário não autenticado' });
      }

      const acknowledged = await auditService.acknowledgeSystemEvent(eventId, userId);

      if (!acknowledged) {
        return res.status(404).json({ message: 'Evento não encontrado' });
      }

      logger.info(`Evento do sistema reconhecido: ${eventId} por ${userId}`);
      res.json({ message: 'Evento reconhecido com sucesso' });
    } catch (error: any) {
      logger.error('Erro ao reconhecer evento do sistema:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  async logSystemEvent(req: Request, res: Response) {
    try {
      const { eventType, title, description, severity, data, source } = req.body;

      const event = await auditService.logSystemEvent(
        eventType,
        title,
        description,
        severity as AuditSeverity,
        data,
        source
      );

      res.status(201).json(event);
    } catch (error: any) {
      logger.error('Erro ao registrar evento do sistema:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Data Exports
  async getDataExports(req: Request, res: Response) {
    try {
      const { requestedBy, limit } = req.query;

      const exports = await auditService.getDataExports(
        requestedBy as string,
        limit ? parseInt(limit as string) : 50
      );

      res.json(exports);
    } catch (error: any) {
      logger.error('Erro ao buscar exports de dados:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Statistics
  async getAuditStatistics(req: Request, res: Response) {
    try {
      const { startDate, endDate } = req.query;

      const start = startDate ? new Date(startDate as string) : new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);
      const end = endDate ? new Date(endDate as string) : new Date();

      const stats = await auditService.getAuditStatistics(start, end);

      res.json({
        period: { startDate: start, endDate: end },
        statistics: stats
      });
    } catch (error: any) {
      logger.error('Erro ao buscar estatísticas de auditoria:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }

  // Maintenance
  async cleanupOldLogs(req: Request, res: Response) {
    try {
      const { daysToKeep } = req.body;

      const deletedCount = await auditService.cleanupOldLogs(
        daysToKeep ? parseInt(daysToKeep) : 365
      );

      logger.info(`Logs antigos limpos: ${deletedCount} registros`);
      res.json({
        message: `${deletedCount} logs antigos foram removidos`,
        deletedCount
      });
    } catch (error: any) {
      logger.error('Erro ao limpar logs antigos:', error);
      res.status(500).json({ message: 'Erro interno do servidor' });
    }
  }
}

export const auditController = new AuditController();