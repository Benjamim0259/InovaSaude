using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

/// <summary>
/// ESF - Estratégia Saúde da Família
/// </summary>
public class ESF
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Endereco { get; set; }

    [StringLength(100)]
    public string? Bairro { get; set; }

    [StringLength(20)]
    public string? Cep { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [ForeignKey("Coordenador")]
    public string? CoordenadorId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "ATIVA";

    public int? CapacidadeAtendimento { get; set; }

    [StringLength(1000)]
    public string? Observacoes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Usuario? Coordenador { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public virtual ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}
