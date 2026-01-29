using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class ImportacaoLote
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    public int TotalRegistros { get; set; }

    [Required]
    public int RegistrosProcessados { get; set; } = 0;

    [Required]
    public int RegistrosErro { get; set; } = 0;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "PROCESSANDO";

    [Required]
    [ForeignKey("Usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Erros { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Usuario Usuario { get; set; } = null!;
}