using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Despesa
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    public DateTime? DataVencimento { get; set; }

    public DateTime? DataPagamento { get; set; }

    [Required]
    [ForeignKey("Categoria")]
    public string CategoriaId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [ForeignKey("ESF")]
    public string EsfId { get; set; } = string.Empty;

    [ForeignKey("Fornecedor")]
    public string? FornecedorId { get; set; }

    [Required]
    [ForeignKey("UsuarioCriacao")]
    public string UsuarioCriacaoId { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [StringLength(50)]
    public string? NumeroNota { get; set; }

    [StringLength(50)]
    public string? NumeroEmpenho { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ESF Esf { get; set; } = null!;

    public virtual Fornecedor? Fornecedor { get; set; }

    public virtual Usuario UsuarioCriacao { get; set; } = null!;

    public virtual ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();

    public virtual ICollection<HistoricoDespesa> HistoricoStatus { get; set; } = new List<HistoricoDespesa>();
}