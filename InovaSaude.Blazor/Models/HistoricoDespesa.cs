using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class HistoricoDespesa
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Despesa")]
    public string DespesaId { get; set; } = string.Empty;

    [StringLength(20)]
    public string? StatusAnterior { get; set; }

    [Required]
    [StringLength(20)]
    public string StatusNovo { get; set; } = string.Empty;

    [ForeignKey("Usuario")]
    public string? UsuarioId { get; set; }

    [StringLength(1000)]
    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Despesa Despesa { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}