using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models;

/// <summary>
/// Pedido de medicamentos de uma UBS para a Farmácia Central
/// </summary>
public class PedidoMedicamento
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Número do pedido (gerado automaticamente)
  /// </summary>
    [Required]
    [StringLength(50)]
    public string NumeroPedido { get; set; } = string.Empty;

    /// <summary>
    /// UBS que está fazendo o pedido
    /// </summary>
  [Required]
  [ForeignKey("UbsSolicitante")]
    public string UbsSolicitanteId { get; set; } = string.Empty;

    /// <summary>
    /// Usuário que criou o pedido
/// </summary>
    [Required]
 [ForeignKey("UsuarioCriacao")]
    public string UsuarioCriacaoId { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do pedido
    /// </summary>
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de necessidade
    /// </summary>
    public DateTime? DataNecessidade { get; set; }

    /// <summary>
    /// Status: PENDENTE, APROVADO, SEPARADO, ENTREGUE, REJEITADO, CANCELADO
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "PENDENTE";

    /// <summary>
/// Prioridade: NORMAL, URGENTE, CRITICA
    /// </summary>
    [StringLength(20)]
    public string Prioridade { get; set; } = "NORMAL";

    /// <summary>
    /// Observações do solicitante
    /// </summary>
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Usuário que aprovou (farmacêutico)
    /// </summary>
    [ForeignKey("UsuarioAprovacao")]
    public string? UsuarioAprovacaoId { get; set; }

    /// <summary>
    /// Data de aprovação
    /// </summary>
    public DateTime? DataAprovacao { get; set; }

/// <summary>
    /// Usuário que entregou
    /// </summary>
    [ForeignKey("UsuarioEntrega")]
 public string? UsuarioEntregaId { get; set; }

    /// <summary>
    /// Data de entrega
    /// </summary>
    public DateTime? DataEntrega { get; set; }

    /// <summary>
    /// Motivo da rejeição (se aplicável)
    /// </summary>
 [StringLength(1000)]
    public string? MotivoRejeicao { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UBS UbsSolicitante { get; set; } = null!;
    public virtual Usuario UsuarioCriacao { get; set; } = null!;
    public virtual Usuario? UsuarioAprovacao { get; set; }
    public virtual Usuario? UsuarioEntrega { get; set; }
    public virtual ICollection<ItemPedidoMedicamento> Itens { get; set; } = new List<ItemPedidoMedicamento>();
}

/// <summary>
/// Item individual de um pedido de medicamento
/// </summary>
public class ItemPedidoMedicamento
{
    [Key]
  public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
  [ForeignKey("PedidoMedicamento")]
    public string PedidoMedicamentoId { get; set; } = string.Empty;

    /// <summary>
 /// Nome do medicamento
    /// </summary>
    [Required]
    [StringLength(500)]
    public string NomeMedicamento { get; set; } = string.Empty;

    /// <summary>
    /// Princípio ativo
    /// </summary>
    [StringLength(500)]
    public string? PrincipioAtivo { get; set; }

    /// <summary>
    /// Concentração/Dosagem
    /// </summary>
    [StringLength(100)]
    public string? Concentracao { get; set; }

    /// <summary>
    /// Forma farmacêutica (comprimido, xarope, etc)
    /// </summary>
    [StringLength(100)]
    public string? FormaFarmaceutica { get; set; }

    /// <summary>
    /// Quantidade solicitada
    /// </summary>
    [Required]
    public int QuantidadeSolicitada { get; set; }

    /// <summary>
    /// Quantidade aprovada (pode ser diferente da solicitada)
    /// </summary>
    public int? QuantidadeAprovada { get; set; }

    /// <summary>
    /// Quantidade entregue
    /// </summary>
    public int? QuantidadeEntregue { get; set; }

    /// <summary>
    /// Justificativa (se quantidade aprovada < solicitada)
    /// </summary>
    [StringLength(1000)]
    public string? Justificativa { get; set; }

    /// <summary>
    /// Código no HORUS (se integrado)
    /// </summary>
    [StringLength(50)]
    public string? CodigoHorus { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PedidoMedicamento PedidoMedicamento { get; set; } = null!;
}

/// <summary>
/// Estoque da Farmácia Central
/// </summary>
public class EstoqueFarmacia
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Nome do medicamento
    /// </summary>
    [Required]
    [StringLength(500)]
    public string NomeMedicamento { get; set; } = string.Empty;

    /// <summary>
    /// Princípio ativo
    /// </summary>
    [StringLength(500)]
    public string? PrincipioAtivo { get; set; }

    /// <summary>
    /// Concentração/Dosagem
    /// </summary>
    [StringLength(100)]
    public string? Concentracao { get; set; }

    /// <summary>
    /// Forma farmacêutica
    /// </summary>
    [StringLength(100)]
    public string? FormaFarmaceutica { get; set; }

  /// <summary>
    /// Código no HORUS
    /// </summary>
    [StringLength(50)]
    public string? CodigoHorus { get; set; }

    /// <summary>
    /// Lote
    /// </summary>
    [StringLength(50)]
    public string? Lote { get; set; }

    /// <summary>
  /// Data de validade
    /// </summary>
    public DateTime? DataValidade { get; set; }

    /// <summary>
    /// Quantidade em estoque
    /// </summary>
    [Required]
    public int QuantidadeAtual { get; set; } = 0;

    /// <summary>
    /// Quantidade mínima (para alertas)
    /// </summary>
    public int QuantidadeMinima { get; set; } = 0;

    /// <summary>
    /// Quantidade máxima
    /// </summary>
 public int? QuantidadeMaxima { get; set; }

    /// <summary>
    /// Localização física no estoque
    /// </summary>
    [StringLength(100)]
    public string? Localizacao { get; set; }

    /// <summary>
    /// Status: DISPONIVEL, BLOQUEADO, VENCIDO, BAIXO_ESTOQUE
    /// </summary>
    [StringLength(20)]
    public string Status { get; set; } = "DISPONIVEL";

    /// <summary>
    /// Última movimentação
    /// </summary>
    public DateTime? UltimaMovimentacao { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

// Navigation properties
    public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}

/// <summary>
/// Movimentações de estoque (entrada/saída)
/// </summary>
public class MovimentacaoEstoque
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("EstoqueFarmacia")]
    public string EstoqueFarmaciaId { get; set; } = string.Empty;

    /// <summary>
    /// Tipo: ENTRADA, SAIDA, AJUSTE, PERDA, VENCIMENTO
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Tipo { get; set; } = string.Empty;

 /// <summary>
    /// Quantidade (positiva para entrada, negativa para saída)
    /// </summary>
 [Required]
    public int Quantidade { get; set; }

 /// <summary>
    /// Quantidade anterior
    /// </summary>
    public int QuantidadeAnterior { get; set; }

    /// <summary>
    /// Quantidade após movimentação
    /// </summary>
    public int QuantidadeApos { get; set; }

    /// <summary>
    /// Motivo da movimentação
    /// </summary>
    [StringLength(1000)]
    public string? Motivo { get; set; }

    /// <summary>
    /// Número do documento (nota fiscal, pedido, etc)
    /// </summary>
    [StringLength(100)]
    public string? NumeroDocumento { get; set; }

    /// <summary>
    /// Pedido relacionado (se for saída para UBS)
    /// </summary>
    [ForeignKey("PedidoMedicamento")]
    public string? PedidoMedicamentoId { get; set; }

    /// <summary>
    /// Usuário responsável
    /// </summary>
    [Required]
    [ForeignKey("Usuario")]
    public string UsuarioId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
  public virtual EstoqueFarmacia EstoqueFarmacia { get; set; } = null!;
    public virtual PedidoMedicamento? PedidoMedicamento { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
}
