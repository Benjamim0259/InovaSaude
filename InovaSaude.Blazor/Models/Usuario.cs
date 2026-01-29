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

    [ForeignKey("UBS")]
    public string? UbsId { get; set; }

    public DateTime? UltimoAcesso { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UBS? Ubs { get; set; }

    public virtual ICollection<UBS> UbsCoordenadas { get; set; } = new List<UBS>();

    public virtual ICollection<Despesa> DespesasCriadas { get; set; } = new List<Despesa>();

    public virtual ICollection<Despesa> DespesasAprovadas { get; set; } = new List<Despesa>();

    public virtual ICollection<LogAuditoria> LogsAuditoria { get; set; } = new List<LogAuditoria>();

    public virtual ICollection<TokenRecuperacaoSenha> TokensRecuperacao { get; set; } = new List<TokenRecuperacaoSenha>();

    public virtual ICollection<PermissaoUsuario> Permissoes { get; set; } = new List<PermissaoUsuario>();
}