using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace InovaSaude.Blazor.Services;

public class WebhookService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        ApplicationDbContext context,
        HttpClient httpClient,
        ILogger<WebhookService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task RegisterWebhookAsync(
        string url,
        WebhookEventType eventType,
        string? secret = null,
        bool isActive = true,
        string? description = null,
        string? createdBy = null)
    {
        var webhook = new Webhook
        {
            Url = url,
            Events = new[] { eventType },
            Status = isActive ? WebhookStatus.ACTIVE : WebhookStatus.INACTIVE,
            Secret = secret,
            Description = description ?? "",
            CreatedBy = createdBy ?? "",
            CreatedAt = DateTime.UtcNow
        };

        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateWebhookAsync(
        string webhookId,
        string? url = null,
        WebhookEventType? eventType = null,
        string? secret = null,
        bool? isActive = null,
        string? description = null)
    {
        var webhook = await _context.Webhooks.FindAsync(webhookId);
        if (webhook == null) return;

        if (!string.IsNullOrEmpty(url)) webhook.Url = url;
        if (eventType.HasValue) webhook.Events = new[] { eventType.Value };
        if (!string.IsNullOrEmpty(secret)) webhook.Secret = secret;
        if (isActive.HasValue) webhook.Status = isActive.Value ? WebhookStatus.ACTIVE : WebhookStatus.INACTIVE;
        if (!string.IsNullOrEmpty(description)) webhook.Description = description;

        webhook.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteWebhookAsync(string webhookId)
    {
        var webhook = await _context.Webhooks.FindAsync(webhookId);
        if (webhook != null)
        {
            _context.Webhooks.Remove(webhook);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Webhook>> GetWebhooksAsync(WebhookEventType? eventType = null, bool? isActive = null)
    {
        var query = _context.Webhooks.AsQueryable();

        if (eventType.HasValue)
            query = query.Where(w => w.Events.Contains(eventType.Value));

        if (isActive.HasValue)
            query = query.Where(w => w.Status == (isActive.Value ? WebhookStatus.ACTIVE : WebhookStatus.INACTIVE));

        return await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
    }

    public async Task TriggerWebhooksAsync(WebhookEventType eventType, object payload)
    {
        var webhooks = await _context.Webhooks
            .Where(w => w.Events.Contains(eventType) && w.Status == WebhookStatus.ACTIVE)
            .ToListAsync();

        foreach (var webhook in webhooks)
        {
            await SendWebhookAsync(webhook, payload);
        }
    }

    private async Task SendWebhookAsync(Webhook webhook, object payload)
    {
        try
        {
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Add signature if secret is provided
            if (!string.IsNullOrEmpty(webhook.Secret))
            {
                var signature = GenerateSignature(jsonPayload, webhook.Secret);
                content.Headers.Add("X-Webhook-Signature", signature);
            }

            var response = await _httpClient.PostAsync(webhook.Url, content);

            // Log the webhook delivery attempt
            var delivery = new WebhookLog
            {
                WebhookId = webhook.Id,
                Event = webhook.Events.FirstOrDefault(),
                Payload = jsonPayload,
                StatusCode = (int)response.StatusCode,
                Response = await response.Content.ReadAsStringAsync(),
                Success = response.IsSuccessStatusCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.WebhookLogs.Add(delivery);
            await _context.SaveChangesAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Webhook delivery failed for {webhook.Url}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending webhook to {webhook.Url}");

            // Log failed delivery
            var delivery = new WebhookLog
            {
                WebhookId = webhook.Id,
                Event = webhook.Events.FirstOrDefault(),
                Payload = JsonSerializer.Serialize(payload),
                Error = ex.Message,
                Success = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.WebhookLogs.Add(delivery);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WebhookLog>> GetWebhookDeliveriesAsync(
        string webhookId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? success = null,
        int page = 1,
        int pageSize = 50)
    {
        var query = _context.WebhookLogs
            .Where(d => d.WebhookId == webhookId);

        if (startDate.HasValue)
            query = query.Where(d => d.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(d => d.CreatedAt <= endDate.Value);

        if (success.HasValue)
            query = query.Where(d => d.Success == success.Value);

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task RetryFailedDeliveriesAsync(string webhookId)
    {
        var failedDeliveries = await _context.WebhookLogs
            .Where(d => d.WebhookId == webhookId && !d.Success)
            .ToListAsync();

        var webhook = await _context.Webhooks.FindAsync(webhookId);
        if (webhook == null) return;

        foreach (var delivery in failedDeliveries)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<object>(delivery.Payload);
                await SendWebhookAsync(webhook, payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Retry failed for webhook delivery {delivery.Id}");
            }
        }
    }

    private string GenerateSignature(string payload, string secret)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    public async Task ValidateWebhookSignatureAsync(string payload, string signature, string secret)
    {
        var expectedSignature = GenerateSignature(payload, secret);
        if (!string.Equals(signature, expectedSignature, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid webhook signature");
        }
    }
}