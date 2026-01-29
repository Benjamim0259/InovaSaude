using System.ComponentModel.DataAnnotations;

namespace InovaSaude.Blazor.Models;

public class Fornecedor
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string RazaoSocial { get; set; } = string.Empty;

    [StringLength(255)]
    public string? NomeFantasia { get; set; }

    [Required]
    [StringLength(20)]
    public string Cnpj { get; set; } = string.Empty;

    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }

    [StringLength(500)]
    public string? Endereco { get; set; }

    [StringLength(100)]
    public string? Bairro { get; set; }

    [StringLength(100)]
    public string? Cidade { get; set; }

    [StringLength(2)]
    public string? Estado { get; set; }

    [StringLength(20)]
    public string? Cep { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? Contato { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "ATIVO";

    [StringLength(1000)]
    public string? Observacoes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}