using System.ComponentModel.DataAnnotations;

namespace InovaSaude.Blazor.Models;

public class SystemEvent
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string Severity { get; set; } = "LOW";

    [StringLength(2000)]
    public string? Data { get; set; }

    [StringLength(255)]
    public string? Source { get; set; }

    public bool Acknowledged { get; set; } = false;

    [StringLength(255)]
    public string? AcknowledgedBy { get; set; }

    public DateTime? AcknowledgedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}