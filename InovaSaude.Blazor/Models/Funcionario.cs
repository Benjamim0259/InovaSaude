using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

/// <summary>
/// Funcionário vinculado a uma UBS (apenas dados cadastrais, não é usuário do sistema)
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
    [ForeignKey("UBS")]
    public string UbsId { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Cargo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UBS Ubs { get; set; } = null!;
}
