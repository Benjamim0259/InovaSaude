using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models.Integrations;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services.Integrations;

/// <summary>
/// Serviço de integração com e-SUS PEC (Prontuário Eletrônico do Cidadão)
/// </summary>
public class EsusPecIntegrationService : ApiExternaServiceBase
{
    public EsusPecIntegrationService(
IHttpClientFactory httpClientFactory,
        ApplicationDbContext context,
        ILogger<EsusPecIntegrationService> logger)
        : base(httpClientFactory, context, logger, "ESUS_PEC")
    {
    }

    /// <summary>
    /// Sincronizar atendimentos do e-SUS PEC
    /// </summary>
    public async Task<bool> SincronizarAtendimentosAsync(
DateTime dataInicio,
    DateTime dataFim,
        string? ubsId = null,
        string? usuarioId = null)
    {
        try
        {
     var config = await ObterConfiguracaoAsync(ubsId);
     if (config == null)
   {
          _logger.LogWarning("Configuração do e-SUS PEC não encontrada");
     return false;
   }

            ConfigurarAutenticacao(config);

            // Endpoint exemplo: /api/v1/atendimentos
         var endpoint = $"/api/v1/atendimentos?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";
       var response = await ExecutarRequisicaoAsync(
              config.Id,
           endpoint,
    HttpMethod.Get,
       usuarioId: usuarioId);

if (response == null || !response.IsSuccessStatusCode)
    {
    return false;
       }

       var atendimentos = await DeserializarRespostaAsync<List<EsusPecAtendimentoDto>>(response);
 if (atendimentos == null || !atendimentos.Any())
     {
       return true; // Sucesso, mas sem dados
     }

            // Salvar atendimentos no banco
  foreach (var atend in atendimentos)
  {
        var existente = await _context.Set<EsusPecAtendimento>()
 .FirstOrDefaultAsync(a => a.IdEsus == atend.Id);

      if (existente != null)
            {
  // Atualizar
            existente.CnsPaciente = atend.CnsPaciente;
        existente.NomePaciente = atend.NomePaciente;
       existente.DataAtendimento = atend.DataAtendimento;
            existente.TipoAtendimento = atend.TipoAtendimento;
            existente.ProcedimentosJson = System.Text.Json.JsonSerializer.Serialize(atend.Procedimentos);
      existente.Cid10 = string.Join(", ", atend.Cid10 ?? new List<string>());
         existente.CnsProfissional = atend.CnsProfissional;
          existente.UpdatedAt = DateTime.UtcNow;
        }
         else
{
   // Criar novo
        var novoAtend = new EsusPecAtendimento
           {
     IdEsus = atend.Id,
         CnsPaciente = atend.CnsPaciente,
          NomePaciente = atend.NomePaciente,
           DataAtendimento = atend.DataAtendimento,
       TipoAtendimento = atend.TipoAtendimento,
           ProcedimentosJson = System.Text.Json.JsonSerializer.Serialize(atend.Procedimentos),
    Cid10 = string.Join(", ", atend.Cid10 ?? new List<string>()),
       CnsProfissional = atend.CnsProfissional,
           UbsId = ubsId
      };
           _context.Set<EsusPecAtendimento>().Add(novoAtend);
      }
            }

       await _context.SaveChangesAsync();
    _logger.LogInformation($"Sincronizados {atendimentos.Count} atendimentos do e-SUS PEC");
            return true;
        }
        catch (Exception ex)
        {
 _logger.LogError(ex, "Erro ao sincronizar atendimentos do e-SUS PEC");
          return false;
        }
    }

 /// <summary>
    /// Obter atendimentos por período
    /// </summary>
public async Task<List<EsusPecAtendimento>> ObterAtendimentosPorPeriodoAsync(
        DateTime dataInicio,
        DateTime dataFim,
        string? ubsId = null)
    {
var query = _context.Set<EsusPecAtendimento>()
     .Where(a => a.DataAtendimento >= dataInicio && a.DataAtendimento <= dataFim);

        if (!string.IsNullOrEmpty(ubsId))
      {
            query = query.Where(a => a.UbsId == ubsId);
        }

  return await query.OrderByDescending(a => a.DataAtendimento).ToListAsync();
    }

    /// <summary>
    /// Obter estatísticas de atendimentos
    /// </summary>
    public async Task<EsusPecEstatisticasDto> ObterEstatisticasAsync(
   DateTime dataInicio,
  DateTime dataFim,
        string? ubsId = null)
    {
        var query = _context.Set<EsusPecAtendimento>()
   .Where(a => a.DataAtendimento >= dataInicio && a.DataAtendimento <= dataFim);

 if (!string.IsNullOrEmpty(ubsId))
      {
     query = query.Where(a => a.UbsId == ubsId);
  }

        var atendimentos = await query.ToListAsync();

     return new EsusPecEstatisticasDto
        {
         TotalAtendimentos = atendimentos.Count,
PacientesUnicos = atendimentos.Select(a => a.CnsPaciente).Distinct().Count(),
   AtendimentosPorTipo = atendimentos
       .GroupBy(a => a.TipoAtendimento ?? "Não especificado")
   .ToDictionary(g => g.Key, g => g.Count()),
AtendimentosPorDia = atendimentos
     .GroupBy(a => a.DataAtendimento.Date)
        .OrderBy(g => g.Key)
    .Select(g => new AtendimentoPorDiaDto { Data = g.Key, Total = g.Count() })
  .ToList()
   };
    }
}

/// <summary>
/// Serviço de integração com NEMESIS (Sistema de Gerenciamento)
/// </summary>
public class NemesisIntegrationService : ApiExternaServiceBase
{
    public NemesisIntegrationService(
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext context,
        ILogger<NemesisIntegrationService> logger)
: base(httpClientFactory, context, logger, "NEMESIS")
    {
  }

    /// <summary>
    /// Sincronizar indicadores do NEMESIS
 /// </summary>
 public async Task<bool> SincronizarIndicadoresAsync(
        string periodoReferencia, // Ex: "2025-01"
    string? ubsId = null,
        string? usuarioId = null)
  {
        try
        {
   var config = await ObterConfiguracaoAsync(ubsId);
            if (config == null)
   {
  _logger.LogWarning("Configuração do NEMESIS não encontrada");
  return false;
            }

   ConfigurarAutenticacao(config);

         // Endpoint exemplo: /api/v1/indicadores
 var endpoint = $"/api/v1/indicadores?periodo={periodoReferencia}";
            var response = await ExecutarRequisicaoAsync(
      config.Id,
          endpoint,
           HttpMethod.Get,
          usuarioId: usuarioId);

   if (response == null || !response.IsSuccessStatusCode)
    {
        return false;
            }

   var indicadores = await DeserializarRespostaAsync<List<NemesisIndicadorDto>>(response);
   if (indicadores == null || !indicadores.Any())
  {
    return true; // Sucesso, mas sem dados
            }

            // Salvar indicadores no banco
foreach (var ind in indicadores)
     {
     var existente = await _context.Set<NemesisIndicador>()
     .FirstOrDefaultAsync(i => i.CodigoIndicador == ind.Codigo && 
    i.PeriodoReferencia == periodoReferencia &&
        i.UbsId == ubsId);

       if (existente != null)
         {
          // Atualizar
       existente.Nome = ind.Nome;
               existente.ValorNumerico = ind.ValorNumerico;
                existente.ValorTexto = ind.ValorTexto;
     existente.Meta = ind.Meta;
          existente.PercentualAlcance = ind.Meta.HasValue && ind.ValorNumerico.HasValue
       ? (ind.ValorNumerico.Value / ind.Meta.Value) * 100
           : null;
    existente.UpdatedAt = DateTime.UtcNow;
         }
      else
      {
        // Criar novo
     var novoInd = new NemesisIndicador
      {
    CodigoIndicador = ind.Codigo,
         Nome = ind.Nome,
        ValorNumerico = ind.ValorNumerico,
            ValorTexto = ind.ValorTexto,
           PeriodoReferencia = periodoReferencia,
             Meta = ind.Meta,
      PercentualAlcance = ind.Meta.HasValue && ind.ValorNumerico.HasValue
        ? (ind.ValorNumerico.Value / ind.Meta.Value) * 100
    : null,
            UbsId = ubsId
     };
       _context.Set<NemesisIndicador>().Add(novoInd);
      }
            }

          await _context.SaveChangesAsync();
_logger.LogInformation($"Sincronizados {indicadores.Count} indicadores do NEMESIS");
  return true;
        }
    catch (Exception ex)
        {
        _logger.LogError(ex, "Erro ao sincronizar indicadores do NEMESIS");
   return false;
  }
    }

    /// <summary>
    /// Obter indicadores por período
    /// </summary>
    public async Task<List<NemesisIndicador>> ObterIndicadoresPorPeriodoAsync(
  string periodoReferencia,
   string? ubsId = null)
  {
        var query = _context.Set<NemesisIndicador>()
            .Where(i => i.PeriodoReferencia == periodoReferencia);

        if (!string.IsNullOrEmpty(ubsId))
{
   query = query.Where(i => i.UbsId == ubsId);
     }

        return await query.OrderBy(i => i.Nome).ToListAsync();
    }

    /// <summary>
    /// Obter indicadores fora da meta
    /// </summary>
    public async Task<List<NemesisIndicador>> ObterIndicadoresForaDaMetaAsync(
     string periodoReferencia,
    string? ubsId = null)
    {
    var query = _context.Set<NemesisIndicador>()
            .Where(i => i.PeriodoReferencia == periodoReferencia &&
     i.Meta.HasValue &&
    i.ValorNumerico.HasValue &&
     i.PercentualAlcance < 80); // Menos de 80% da meta

 if (!string.IsNullOrEmpty(ubsId))
     {
          query = query.Where(i => i.UbsId == ubsId);
}

        return await query.OrderBy(i => i.PercentualAlcance).ToListAsync();
    }
}

// DTOs
public class EsusPecAtendimentoDto
{
    public string Id { get; set; } = string.Empty;
    public string CnsPaciente { get; set; } = string.Empty;
    public string? NomePaciente { get; set; }
    public DateTime DataAtendimento { get; set; }
    public string? TipoAtendimento { get; set; }
    public List<string>? Procedimentos { get; set; }
    public List<string>? Cid10 { get; set; }
    public string? CnsProfissional { get; set; }
}

public class EsusPecEstatisticasDto
{
    public int TotalAtendimentos { get; set; }
    public int PacientesUnicos { get; set; }
    public Dictionary<string, int> AtendimentosPorTipo { get; set; } = new();
    public List<AtendimentoPorDiaDto> AtendimentosPorDia { get; set; } = new();
}

public class AtendimentoPorDiaDto
{
    public DateTime Data { get; set; }
    public int Total { get; set; }
}

public class NemesisIndicadorDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal? ValorNumerico { get; set; }
    public string? ValorTexto { get; set; }
    public decimal? Meta { get; set; }
}
