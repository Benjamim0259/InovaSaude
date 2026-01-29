using System.ComponentModel.DataAnnotations;

namespace InovaSaude.Blazor.Models;

public class Integration
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "INACTIVE";

    [StringLength(2000)]
    public string Configuration { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Settings { get; set; }

    [Required]
    [StringLength(255)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(255)]
    public string? UpdatedBy { get; set; }

    public DateTime? LastSyncAt { get; set; }

    [StringLength(1000)]
    public string? LastSyncError { get; set; }

    public int SyncCount { get; set; } = 0;

    public int ErrorCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<IntegrationLog> Logs { get; set; } = new List<IntegrationLog>();

    public virtual ICollection<ExternalSync> Syncs { get; set; } = new List<ExternalSync>();

    public virtual ICollection<ApiEndpoint> Endpoints { get; set; } = new List<ApiEndpoint>();

    public virtual ICollection<PaymentTransaction> Payments { get; set; } = new List<PaymentTransaction>();
}