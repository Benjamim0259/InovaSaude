using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class WorkflowInstance
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Workflow")]
    public string WorkflowId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string EntityId { get; set; } = string.Empty;

    [Required]
    public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.PENDING;

    [Required]
    public string InitiatedBy { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Context { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Workflow Workflow { get; set; } = null!;

    public virtual ICollection<WorkflowStepInstance> StepInstances { get; set; } = new List<WorkflowStepInstance>();
}