using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class PermissaoUsuario
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    [Required]
    public Permissao Permissao { get; set; }

    public DateTime ConcedidaEm { get; set; } = DateTime.UtcNow;

    [StringLength(255)]
    public string? ConcedidaPor { get; set; }

    // Navigation property
    public virtual Usuario Usuario { get; set; } = null!;
}