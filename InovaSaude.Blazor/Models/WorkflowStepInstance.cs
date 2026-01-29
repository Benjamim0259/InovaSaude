using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class WorkflowStepInstance
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("WorkflowInstance")]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string StepId { get; set; } = string.Empty;

    [Required]
    public WorkflowStepStatus Status { get; set; } = WorkflowStepStatus.PENDING;

    [StringLength(255)]
    public string? AssignedTo { get; set; }

    [StringLength(255)]
    public string? CompletedBy { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    public ApprovalAction? Action { get; set; }

    [StringLength(2000)]
    public string? ActionData { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}