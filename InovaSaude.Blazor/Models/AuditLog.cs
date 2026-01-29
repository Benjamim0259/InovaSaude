using System.ComponentModel.DataAnnotations;

namespace InovaSaude.Blazor.Models;

public class AuditLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [StringLength(255)]
    public string? EntityId { get; set; }

    [StringLength(255)]
    public string? UserId { get; set; }

    [StringLength(255)]
    public string? UserEmail { get; set; }

    [StringLength(255)]
    public string? UserName { get; set; }

    [StringLength(4000)]
    public string? OldValues { get; set; }

    [StringLength(4000)]
    public string? NewValues { get; set; }

    [StringLength(2000)]
    public string? Changes { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(255)]
    public string? SessionId { get; set; }

    [StringLength(20)]
    public string Severity { get; set; } = "LOW";

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(2000)]
    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}