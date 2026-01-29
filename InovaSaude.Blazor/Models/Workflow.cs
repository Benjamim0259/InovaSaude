using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Workflow
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public WorkflowStatus Status { get; set; } = WorkflowStatus.DRAFT;

    [Required]
    public WorkflowTriggerType TriggerType { get; set; } = WorkflowTriggerType.MANUAL;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? TriggerConditions { get; set; }

    public int Version { get; set; } = 1;

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(255)]
    public string? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();

    public virtual ICollection<WorkflowInstance> Instances { get; set; } = new List<WorkflowInstance>();
}