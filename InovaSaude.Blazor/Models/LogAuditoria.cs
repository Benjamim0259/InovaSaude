using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class LogAuditoria
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("Usuario")]
    public string? UsuarioId { get; set; }

    [Required]
    [StringLength(100)]
    public string Acao { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Entidade { get; set; } = string.Empty;

    [StringLength(255)]
    public string? EntidadeId { get; set; }

    [StringLength(4000)]
    public string? DadosAntigos { get; set; }

    [StringLength(4000)]
    public string? DadosNovos { get; set; }

    [StringLength(45)]
    public string? Ip { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Usuario? Usuario { get; set; }
}