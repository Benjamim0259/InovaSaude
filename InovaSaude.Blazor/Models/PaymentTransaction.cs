using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class PaymentTransaction
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("Integration")]
    public string? IntegrationId { get; set; }

    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string TransactionId { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "BRL";

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? RelatedEntityId { get; set; }

    [StringLength(100)]
    public string? RelatedEntityType { get; set; }

    [StringLength(2000)]
    public string? PaymentData { get; set; }

    [StringLength(2000)]
    public string? Metadata { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Integration? Integration { get; set; }
}