import { Request, Response, NextFunction } from 'express';
import { auditService } from '../../modules/audit/audit.service';
import { AuditAction, AuditEntityType, AuditSeverity } from '../../modules/audit/audit.entity';
import logger from '../../config/logger';

interface AuditOptions {
  entityType?: AuditEntityType;
  action?: AuditAction;
  severity?: AuditSeverity;
  skipOnSuccess?: boolean;
  skipOnError?: boolean;
}

export const auditMiddleware = (options: AuditOptions = {}) => {
  return async (req: Request, res: Response, next: NextFunction) => {
    const startTime = Date.now();
    const originalSend = res.send;
    let responseData: any = null;
    let responseStatus = 200;

    // Override res.send to capture response data
    res.send = function(data: any) {
      responseData = data;
      responseStatus = res.statusCode;
      return originalSend.call(this, data);
    };

    // Continue with request processing
    try {
      await next();

      // Log successful requests (unless configured to skip)
      if (!options.skipOnSuccess) {
        await logAuditEvent(req, res, responseData, responseStatus, startTime, false, options);
      }
    } catch (error) {
      // Log error requests (unless configured to skip)
      if (!options.skipOnError) {
        await logAuditEvent(req, res, responseData, responseStatus, startTime, true, options, error);
      }
      throw error;
    }
  };
};

async function logAuditEvent(
  req: Request,
  res: Response,
  responseData: any,
  responseStatus: number,
  startTime: number,
  isError: boolean,
  options: AuditOptions,
  error?: any
) {
  try {
    const userId = req.user?.id;
    const userEmail = req.user?.email;
    const userName = req.user?.nome;

    // Skip if no user (public endpoints)
    if (!userId) return;

    // Determine entity type from URL
    const entityType = options.entityType || extractEntityTypeFromUrl(req.url);

    // Determine action from HTTP method
    const action = options.action || mapHttpMethodToAction(req.method);

    // Determine severity
    let severity = options.severity || AuditSeverity.LOW;
    if (isError) severity = AuditSeverity.MEDIUM;
    if (responseStatus >= 500) severity = AuditSeverity.HIGH;
    if (responseStatus >= 400 && responseStatus < 500) severity = AuditSeverity.MEDIUM;

    // Extract entity ID from URL or body
    const entityId = extractEntityId(req);

    // Prepare metadata
    const metadata: any = {
      method: req.method,
      url: req.url,
      userAgent: req.get('User-Agent'),
      ip: req.ip || req.connection.remoteAddress,
      duration: Date.now() - startTime,
      responseStatus,
      timestamp: new Date().toISOString()
    };

    // Add request body for sensitive operations (excluding passwords)
    if (['POST', 'PUT', 'PATCH'].includes(req.method) && req.body) {
      const sanitizedBody = { ...req.body };
      if (sanitizedBody.password) delete sanitizedBody.password;
      if (sanitizedBody.confirmPassword) delete sanitizedBody.confirmPassword;
      metadata.requestBody = sanitizedBody;
    }

    // Add query parameters
    if (Object.keys(req.query).length > 0) {
      metadata.queryParams = req.query;
    }

    // Add error details if present
    if (error) {
      metadata.error = {
        message: error.message,
        stack: error.stack
      };
    }

    // Determine description
    let description = `${action} ${entityType}`;
    if (entityId) description += ` ${entityId}`;
    if (isError) description += ' - ERROR';
    description += ` (${responseStatus})`;

    await auditService.logActivity({
      action,
      entityType,
      entityId,
      userId,
      userEmail,
      userName,
      ipAddress: req.ip,
      userAgent: req.get('User-Agent'),
      sessionId: (req as any).sessionId,
      severity,
      description,
      metadata
    });

  } catch (auditError) {
    // Don't let audit logging break the main flow
    logger.error('Failed to log audit event:', auditError);
  }
}

function extractEntityTypeFromUrl(url: string): AuditEntityType {
  const urlParts = url.split('/').filter(part => part.length > 0);

  // Map common URL patterns to entity types
  const entityMap: { [key: string]: AuditEntityType } = {
    'despesas': AuditEntityType.DESPESA,
    'ubs': AuditEntityType.UBS,
    'usuarios': AuditEntityType.USER,
    'auth': AuditEntityType.USER,
    'webhooks': AuditEntityType.WEBHOOK,
    'workflows': AuditEntityType.WORKFLOW,
    'integrations': AuditEntityType.SYSTEM,
    'audit': AuditEntityType.SYSTEM,
    'relatorios': AuditEntityType.SYSTEM,
    'importacao': AuditEntityType.SYSTEM,
    'permissoes': AuditEntityType.PERMISSION,
    'dashboard': AuditEntityType.SYSTEM
  };

  for (const part of urlParts) {
    if (entityMap[part]) {
      return entityMap[part];
    }
  }

  return AuditEntityType.SYSTEM;
}

function mapHttpMethodToAction(method: string): AuditAction {
  switch (method.toUpperCase()) {
    case 'POST': return AuditAction.CREATE;
    case 'PUT':
    case 'PATCH': return AuditAction.UPDATE;
    case 'DELETE': return AuditAction.DELETE;
    case 'GET': return AuditAction.VIEW;
    default: return AuditAction.VIEW;
  }
}

function extractEntityId(req: Request): string | undefined {
  const urlParts = req.url.split('/').filter(part => part.length > 0);

  // Look for UUID pattern in URL
  const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

  for (const part of urlParts) {
    if (uuidRegex.test(part)) {
      return part;
    }
  }

  // Check body for ID
  if (req.body && req.body.id) {
    return req.body.id;
  }

  return undefined;
}

// Specific audit middlewares for different scenarios
export const auditCreate = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.CREATE, severity: AuditSeverity.LOW });

export const auditUpdate = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.UPDATE, severity: AuditSeverity.LOW });

export const auditDelete = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.DELETE, severity: AuditSeverity.MEDIUM });

export const auditView = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.VIEW, severity: AuditSeverity.LOW });

export const auditExport = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.EXPORT, severity: AuditSeverity.LOW });

export const auditImport = (entityType: AuditEntityType) =>
  auditMiddleware({ entityType, action: AuditAction.IMPORT, severity: AuditSeverity.MEDIUM });

export const auditAuth = () =>
  auditMiddleware({
    entityType: AuditEntityType.USER,
    severity: AuditSeverity.LOW,
    skipOnError: false
  });