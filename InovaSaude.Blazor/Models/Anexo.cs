using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Anexo
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Despesa")]
    public string DespesaId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string CaminhoArquivo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string TipoArquivo { get; set; } = string.Empty;

    [Required]
    public long Tamanho { get; set; }

    [StringLength(1000)]
    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Despesa Despesa { get; set; } = null!;
}