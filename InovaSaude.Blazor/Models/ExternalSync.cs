using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class ExternalSync
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Integration")]
    public string IntegrationId { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Direction { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "PENDING";

    public int RecordsTotal { get; set; } = 0;

    public int RecordsProcessed { get; set; } = 0;

    public int RecordsFailed { get; set; } = 0;

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    [StringLength(4000)]
    public string? SyncData { get; set; }

    [Required]
    [StringLength(255)]
    public string InitiatedBy { get; set; } = string.Empty;

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Integration Integration { get; set; } = null!;
}