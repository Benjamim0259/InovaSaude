using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class AuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActivityAsync(
        string action,
        string entityType,
        string? entityId,
        string? userId,
        string? userEmail,
        string? userName,
        string? oldValues = null,
        string? newValues = null,
        string? changes = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null,
        string severity = "LOW",
        string? description = null,
        string? metadata = null)
    {
        var auditLog = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            UserEmail = userEmail,
            UserName = userName,
            OldValues = oldValues,
            NewValues = newValues,
            Changes = changes,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            SessionId = sessionId,
            Severity = severity,
            Description = description,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAuditLogsAsync(
        string? entityType = null,
        string? entityId = null,
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? action = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrEmpty(entityId))
            query = query.Where(a => a.EntityId == entityId);

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(a => a.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(a => a.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.CreatedAt <= endDate.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action.Contains(action));

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetUserActivityAsync(string userId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);

        return await _context.AuditLogs
            .Where(a => a.UserId == userId && a.CreatedAt >= startDate)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetEntityHistoryAsync(string entityType, string entityId)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetActivitySummaryAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.AuditLogs
            .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
            .GroupBy(a => a.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count);
    }

    public async Task<List<SystemEvent>> GetSystemEventsAsync(bool onlyUnacknowledged = false, int limit = 100)
    {
        var query = _context.SystemEvents.AsQueryable();

        if (onlyUnacknowledged)
            query = query.Where(e => !e.Acknowledged);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AcknowledgeSystemEventAsync(string eventId, string acknowledgedBy)
    {
        var systemEvent = await _context.SystemEvents.FindAsync(eventId);
        if (systemEvent != null)
        {
            systemEvent.Acknowledged = true;
            systemEvent.AcknowledgedBy = acknowledgedBy;
            systemEvent.AcknowledgedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateSystemEventAsync(
        string eventType,
        string title,
        string description,
        string severity = "LOW",
        string? data = null,
        string? source = null)
    {
        var systemEvent = new SystemEvent
        {
            EventType = eventType,
            Title = title,
            Description = description,
            Severity = severity,
            Data = data,
            Source = source,
            CreatedAt = DateTime.UtcNow
        };

        _context.SystemEvents.Add(systemEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<List<EntityVersion>> GetEntityVersionsAsync(string entityType, string entityId)
    {
        return await _context.EntityVersions
            .Where(v => v.EntityType == entityType && v.EntityId == entityId)
            .OrderByDescending(v => v.Version)
            .ToListAsync();
    }

    public async Task CreateEntityVersionAsync(
        string entityType,
        string entityId,
        int version,
        string data,
        string changedBy,
        string? changedByEmail = null,
        string? changeReason = null)
    {
        var entityVersion = new EntityVersion
        {
            EntityType = entityType,
            EntityId = entityId,
            Version = version,
            Data = data,
            ChangedBy = changedBy,
            ChangedByEmail = changedByEmail,
            ChangeReason = changeReason,
            CreatedAt = DateTime.UtcNow
        };

        _context.EntityVersions.Add(entityVersion);
        await _context.SaveChangesAsync();
    }
}