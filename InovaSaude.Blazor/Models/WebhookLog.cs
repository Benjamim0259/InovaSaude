using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class WebhookLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Webhook")]
    public string WebhookId { get; set; } = string.Empty;

    [Required]
    public WebhookEventType Event { get; set; }

    [Required]
    [StringLength(4000)]
    public string Payload { get; set; } = string.Empty;

    [Required]
    public int StatusCode { get; set; }

    [StringLength(4000)]
    public string? Response { get; set; }

    [StringLength(1000)]
    public string? Error { get; set; }

    public int RetryCount { get; set; } = 0;

    public bool Success { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Webhook Webhook { get; set; } = null!;
}