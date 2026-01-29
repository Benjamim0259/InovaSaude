using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class TokenRecuperacaoSenha
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiradoEm { get; set; }

    public DateTime? UtilizadoEm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Usuario Usuario { get; set; } = null!;
}