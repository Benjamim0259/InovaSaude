using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class IntegrationService
{
    private readonly ApplicationDbContext _context;

    public IntegrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Integration>> GetAllIntegrationsAsync()
    {
        return await _context.Integrations
            .Include(i => i.Logs.OrderByDescending(l => l.CreatedAt).Take(5))
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Integration?> GetIntegrationByIdAsync(string id)
    {
        return await _context.Integrations
            .Include(i => i.Logs)
            .Include(i => i.Syncs)
            .Include(i => i.Endpoints)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task CreateIntegrationAsync(Integration integration)
    {
        _context.Integrations.Add(integration);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIntegrationAsync(Integration integration)
    {
        _context.Integrations.Update(integration);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteIntegrationAsync(string id)
    {
        var integration = await _context.Integrations.FindAsync(id);
        if (integration != null)
        {
            _context.Integrations.Remove(integration);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Webhook>> GetAllWebhooksAsync()
    {
        return await _context.Webhooks
            .Include(w => w.Logs.OrderByDescending(l => l.CreatedAt).Take(10))
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task CreateWebhookAsync(Webhook webhook)
    {
        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PaymentTransaction>> GetPaymentTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.PaymentTransactions
            .Include(p => p.Integration)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(p => p.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(p => p.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ExternalSync>> GetSyncHistoryAsync(string? integrationId = null)
    {
        var query = _context.ExternalSyncs
            .Include(s => s.Integration)
            .AsQueryable();

        if (!string.IsNullOrEmpty(integrationId))
            query = query.Where(s => s.IntegrationId == integrationId);

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task LogIntegrationActivityAsync(string integrationId, string operation, string status, string? requestData = null, string? responseData = null, string? errorMessage = null)
    {
        var log = new IntegrationLog
        {
            IntegrationId = integrationId,
            Operation = operation,
            Status = status,
            RequestData = requestData,
            ResponseData = responseData,
            ErrorMessage = errorMessage,
            CreatedAt = DateTime.UtcNow
        };

        _context.IntegrationLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIntegrationStatsAsync(string integrationId)
    {
        var integration = await _context.Integrations.FindAsync(integrationId);
        if (integration == null) return;

        integration.LastSyncAt = DateTime.UtcNow;
        integration.SyncCount = await _context.IntegrationLogs
            .CountAsync(l => l.IntegrationId == integrationId && l.Status == "SUCCESS");

        integration.ErrorCount = await _context.IntegrationLogs
            .CountAsync(l => l.IntegrationId == integrationId && l.Status == "ERROR");

        await _context.SaveChangesAsync();
    }
}