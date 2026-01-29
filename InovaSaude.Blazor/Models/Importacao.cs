using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

public class Importacao
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(255)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "PENDING";

    [StringLength(1000)]
    public string? Descricao { get; set; }

    [StringLength(4000)]
    public string? Erro { get; set; }

    public int? TotalRegistros { get; set; }

    public int? RegistrosProcessados { get; set; } = 0;

    public int? RegistrosErro { get; set; } = 0;

    [ForeignKey("Usuario")]
    public string? CriadoPor { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? AtualizadoEm { get; set; }

    // Navigation properties
    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<ImportacaoItem> Itens { get; set; } = new List<ImportacaoItem>();
}

public class ImportacaoItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Importacao")]
    public string ImportacaoId { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string DadosOriginais { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "PENDING";

    [StringLength(1000)]
    public string? Erro { get; set; }

    [StringLength(100)]
    public string? EntidadeCriadaId { get; set; }

    [StringLength(50)]
    public string? EntidadeTipo { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Importacao Importacao { get; set; } = null!;
}

public class WebhookDelivery
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("Webhook")]
    public string WebhookId { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Payload { get; set; } = string.Empty;

    public int? ResponseStatusCode { get; set; }

    [StringLength(4000)]
    public string? ResponseBody { get; set; }

    [StringLength(1000)]
    public string? Error { get; set; }

    public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;

    public bool Success { get; set; } = false;

    // Navigation property
    public virtual Webhook Webhook { get; set; } = null!;
}