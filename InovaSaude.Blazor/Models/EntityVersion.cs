using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

[Table("entity_versions")]
public class EntityVersion
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string EntityId { get; set; } = string.Empty;

    [Required]
    public int Version { get; set; }

    [Required]
    [StringLength(4000)]
    public string Data { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string ChangedBy { get; set; } = string.Empty;

    [StringLength(255)]
    public string? ChangedByEmail { get; set; }

    [StringLength(500)]
    public string? ChangeReason { get; set; }

    public bool IsActive { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}