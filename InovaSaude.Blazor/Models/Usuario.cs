using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Usuario
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    public PerfilUsuario Perfil { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "ATIVO";

    [StringLength(20)]
    public string? Telefone { get; set; }

    [ForeignKey("ESF")]
    public string? EsfId { get; set; }

    public DateTime? UltimoAcesso { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ESF? Esf { get; set; }

    public virtual ICollection<ESF> EsfCoordenadas { get; set; } = new List<ESF>();

    public virtual ICollection<Despesa> DespesasCriadas { get; set; } = new List<Despesa>();

    public virtual ICollection<LogAuditoria> LogsAuditoria { get; set; } = new List<LogAuditoria>();

    public virtual ICollection<TokenRecuperacaoSenha> TokensRecuperacao { get; set; } = new List<TokenRecuperacaoSenha>();

    public virtual ICollection<PermissaoUsuario> Permissoes { get; set; } = new List<PermissaoUsuario>();
}