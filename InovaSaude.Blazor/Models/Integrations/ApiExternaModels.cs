using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaSaude.Blazor.Models.Integrations;

/// <summary>
/// Configuração de API externa (HORUS, e-SUS PEC, NEMESIS)
/// </summary>
public class ApiExterna
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Nome da API: HORUS, ESUS_PEC, NEMESIS
    /// </summary>
    [Required]
    [StringLength(50)]
 public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// URL base da API
    /// </summary>
    [Required]
  [StringLength(500)]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de autenticação: Bearer, ApiKey, OAuth2, Basic
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoAutenticacao { get; set; } = "Bearer";

    /// <summary>
    /// Token/API Key (criptografado)
    /// </summary>
    [StringLength(1000)]
    public string? Token { get; set; }

    /// <summary>
    /// Client ID (para OAuth2)
    /// </summary>
    [StringLength(255)]
    public string? ClientId { get; set; }

    /// <summary>
    /// Client Secret (criptografado, para OAuth2)
    /// </summary>
    [StringLength(1000)]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Timeout em segundos
    /// </summary>
    public int TimeoutSegundos { get; set; } = 30;

    /// <summary>
    /// Máximo de tentativas em caso de falha
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Status: ATIVA, INATIVA, ERRO
    /// </summary>
    [StringLength(20)]
    public string Status { get; set; } = "ATIVA";

    /// <summary>
    /// Última sincronização bem-sucedida
    /// </summary>
    public DateTime? UltimaSincronizacao { get; set; }

    /// <summary>
    /// Última tentativa de sincronização
    /// </summary>
    public DateTime? UltimaTentativa { get; set; }

    /// <summary>
    /// Último erro registrado
    /// </summary>
    [StringLength(2000)]
    public string? UltimoErro { get; set; }

    /// <summary>
    /// Total de sincronizações bem-sucedidas
    /// </summary>
    public int TotalSincronizacoes { get; set; } = 0;

    /// <summary>
    /// Total de erros
    /// </summary>
    public int TotalErros { get; set; } = 0;

    /// <summary>
    /// Configurações adicionais (JSON)
    /// </summary>
    [StringLength(4000)]
    public string? ConfiguracoesJson { get; set; }

    /// <summary>
/// ESF associada (opcional, para configurações por unidade)
    /// </summary>
    [ForeignKey("ESF")]
    public string? EsfId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

 // Navigation properties
  public virtual ESF? Esf { get; set; }
    public virtual ICollection<LogIntegracaoApi> Logs { get; set; } = new List<LogIntegracaoApi>();
}

/// <summary>
/// Log de integrações com APIs externas
/// </summary>
public class LogIntegracaoApi
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("ApiExterna")]
    public string ApiExternaId { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint chamado
    /// </summary>
 [Required]
    [StringLength(500)]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Método HTTP: GET, POST, PUT, DELETE
    /// </summary>
    [Required]
    [StringLength(10)]
    public string MetodoHttp { get; set; } = string.Empty;

    /// <summary>
    /// Status HTTP da resposta
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Sucesso ou falha
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Tempo de resposta em milissegundos
    /// </summary>
    public long? TempoRespostaMs { get; set; }

    /// <summary>
    /// Request payload (primeiros 2000 caracteres)
    /// </summary>
    [StringLength(2000)]
    public string? RequestPayload { get; set; }

    /// <summary>
    /// Response payload (primeiros 2000 caracteres)
 /// </summary>
    [StringLength(2000)]
    public string? ResponsePayload { get; set; }

    /// <summary>
    /// Mensagem de erro (se houver)
    /// </summary>
    [StringLength(2000)]
    public string? MensagemErro { get; set; }

    /// <summary>
    /// Tentativa número (para retries)
    /// </summary>
    public int NumeroTentativa { get; set; } = 1;

    /// <summary>
    /// Usuário que iniciou a integração
    /// </summary>
    [ForeignKey("Usuario")]
    public string? UsuarioId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApiExterna ApiExterna { get; set; } = null!;
    public virtual Usuario? Usuario { get; set; }
}

/// <summary>
/// Dados sincronizados do HORUS (Medicamentos)
/// </summary>
public class HorusMedicamento
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Código do medicamento no HORUS
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CodigoHorus { get; set; } = string.Empty;

    /// <summary>
    /// Nome do medicamento
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Princípio ativo
    /// </summary>
  [StringLength(500)]
    public string? PrincipioAtivo { get; set; }

    /// <summary>
    /// Concentração
    /// </summary>
    [StringLength(100)]
    public string? Concentracao { get; set; }

    /// <summary>
    /// Forma farmacêutica
    /// </summary>
    [StringLength(100)]
    public string? FormaFarmaceutica { get; set; }

    /// <summary>
    /// Quantidade em estoque
    /// </summary>
    public int QuantidadeEstoque { get; set; } = 0;

    /// <summary>
    /// Quantidade mínima
    /// </summary>
    public int QuantidadeMinima { get; set; } = 0;

    /// <summary>
    /// Custo unitário do medicamento
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal CustoUnitario { get; set; } = 0;

    /// <summary>
    /// Custo total (QuantidadeEstoque * CustoUnitario)
    /// </summary>
    [NotMapped]
    public decimal CustoTotal => QuantidadeEstoque * CustoUnitario;

    /// <summary>
    /// Lote do medicamento
    /// </summary>
    [StringLength(50)]
    public string? Lote { get; set; }

    /// <summary>
    /// Data de validade
    /// </summary>
    public DateTime? DataValidade { get; set; }

    /// <summary>
    /// Última atualização do HORUS
    /// </summary>
    public DateTime? UltimaAtualizacaoHorus { get; set; }

    [ForeignKey("ESF")]
    public string? EsfId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ESF? Esf { get; set; }
}

/// <summary>
/// Dados sincronizados do e-SUS PEC (Atendimentos)
/// </summary>
public class EsusPecAtendimento
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID do atendimento no e-SUS PEC
    /// </summary>
    [Required]
[StringLength(100)]
    public string IdEsus { get; set; } = string.Empty;

    /// <summary>
    /// CNS do paciente
 /// </summary>
  [Required]
    [StringLength(15)]
    public string CnsPaciente { get; set; } = string.Empty;

    /// <summary>
    /// Nome do paciente
    /// </summary>
    [StringLength(255)]
    public string? NomePaciente { get; set; }

    /// <summary>
 /// Data do atendimento
    /// </summary>
    public DateTime DataAtendimento { get; set; }

    /// <summary>
    /// Tipo de atendimento
    /// </summary>
    [StringLength(100)]
    public string? TipoAtendimento { get; set; }

    /// <summary>
    /// Procedimentos realizados (JSON)
/// </summary>
 [StringLength(4000)]
    public string? ProcedimentosJson { get; set; }

    /// <summary>
    /// CID-10 registrados
    /// </summary>
    [StringLength(500)]
    public string? Cid10 { get; set; }

    /// <summary>
    /// Profissional responsável (CNS)
    /// </summary>
    [StringLength(15)]
    public string? CnsProfissional { get; set; }

    [ForeignKey("ESF")]
    public string? EsfId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ESF? Esf { get; set; }
}

/// <summary>
/// Dados sincronizados do NEMESIS (Indicadores)
/// </summary>
public class NemesisIndicador
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Código do indicador no NEMESIS
    /// </summary>
    [Required]
    [StringLength(50)]
    public string CodigoIndicador { get; set; } = string.Empty;

    /// <summary>
    /// Nome do indicador
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Valor numérico
    /// </summary>
 public decimal? ValorNumerico { get; set; }

    /// <summary>
    /// Valor texto
    /// </summary>
    [StringLength(500)]
    public string? ValorTexto { get; set; }

    /// <summary>
    /// Período de referência (ex: 2025-01)
    /// </summary>
    [StringLength(20)]
    public string? PeriodoReferencia { get; set; }

    /// <summary>
    /// Meta estabelecida
    /// </summary>
    public decimal? Meta { get; set; }

    /// <summary>
    /// Porcentagem de alcance da meta
    /// </summary>
    public decimal? PercentualAlcance { get; set; }

    [ForeignKey("ESF")]
    public string? EsfId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ESF? Esf { get; set; }
}


