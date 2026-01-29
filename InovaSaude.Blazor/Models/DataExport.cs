using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class DataExport
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string ExportType { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Filters { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Data { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? FileUrl { get; set; }

    public long FileSize { get; set; }

    [Required]
    [StringLength(255)]
    public string RequestedBy { get; set; } = string.Empty;

    [StringLength(255)]
    public string? RequestedByEmail { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? RecordCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}