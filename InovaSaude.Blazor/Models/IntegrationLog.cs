using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class IntegrationLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Integration")]
    public string IntegrationId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Operation { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "PENDING";

    [StringLength(4000)]
    public string? RequestData { get; set; }

    [StringLength(4000)]
    public string? ResponseData { get; set; }

    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    public int? RecordsProcessed { get; set; }

    public int? RecordsFailed { get; set; }

    public long? Duration { get; set; }

    [StringLength(2000)]
    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Integration Integration { get; set; } = null!;
}