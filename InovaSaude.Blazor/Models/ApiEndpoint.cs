using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class ApiEndpoint
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Integration")]
    public string IntegrationId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Method { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Path { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(2000)]
    public string? RequestSchema { get; set; }

    [StringLength(2000)]
    public string? ResponseSchema { get; set; }

    [StringLength(1000)]
    public string? Headers { get; set; }

    public bool Active { get; set; } = true;

    public int CallCount { get; set; } = 0;

    public int ErrorCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Integration Integration { get; set; } = null!;
}