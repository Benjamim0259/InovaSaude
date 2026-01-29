using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class WorkflowStep
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Workflow")]
    public string WorkflowId { get; set; } = string.Empty;

    [Required]
    public int Order { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public WorkflowStepType Type { get; set; }

    [Required]
    [StringLength(2000)]
    public string Configuration { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Conditions { get; set; }

    [StringLength(255)]
    public string? AssignedTo { get; set; }

    public int? TimeoutHours { get; set; }

    public bool Required { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Workflow Workflow { get; set; } = null!;
}