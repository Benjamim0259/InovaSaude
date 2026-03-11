using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

/// <summary>
/// Funcionário vinculado a uma ESF (apenas dados cadastrais, não é usuário do sistema)
/// </summary>
public class Funcionario
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Salario { get; set; }

    [Required]
    [ForeignKey("ESF")]
    public string EsfId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Cargo { get; set; } = "Outros";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ESF Esf { get; set; } = null!;
}
