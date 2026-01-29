using System.ComponentModel.DataAnnotations;

namespace InovaSaude.Blazor.Models;

public class Categoria
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    public decimal? OrcamentoMensal { get; set; }

    [StringLength(50)]
    public string? Cor { get; set; }

    [StringLength(50)]
    public string? Icone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}