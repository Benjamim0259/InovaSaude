using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Webhook
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    [Url]
    public string Url { get; set; } = string.Empty;

    public WebhookEventType[] Events { get; set; } = Array.Empty<WebhookEventType>();

    [Required]
    public WebhookStatus Status { get; set; } = WebhookStatus.ACTIVE;

    [StringLength(255)]
    public string? Secret { get; set; }

    public int RetryCount { get; set; } = 3;

    public int Timeout { get; set; } = 5000;

    [StringLength(2000)]
    public string? Headers { get; set; }

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<WebhookLog> Logs { get; set; } = new List<WebhookLog>();
}