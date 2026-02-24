using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using InovaSaude.Blazor.Models.Integrations;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace InovaSaude.Blazor.Services.Integrations;

/// <summary>
/// Serviço base para integrações com APIs externas
/// </summary>
public abstract class ApiExternaServiceBase
{
    protected readonly HttpClient _httpClient;
    protected readonly ApplicationDbContext _context;
    protected readonly ILogger _logger;
 protected readonly string _apiNome;

    protected ApiExternaServiceBase(
   IHttpClientFactory httpClientFactory,
        ApplicationDbContext context,
     ILogger logger,
        string apiNome)
    {
      _httpClient = httpClientFactory.CreateClient();
    _context = context;
 _logger = logger;
      _apiNome = apiNome;
    }

    /// <summary>
    /// Obter configuração da API
    /// </summary>
    protected async Task<ApiExterna?> ObterConfiguracaoAsync(string? esfId = null)
    {
   var query = _context.Set<ApiExterna>()
    .Where(a => a.Nome == _apiNome && a.Status == "ATIVA");

     if (!string.IsNullOrEmpty(esfId))
     {
    query = query.Where(a => a.EsfId == esfId || a.EsfId == null);
      }

   return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Configurar autenticação no HttpClient
    /// </summary>
    protected void ConfigurarAutenticacao(ApiExterna config)
    {
        _httpClient.DefaultRequestHeaders.Clear();
_httpClient.BaseAddress = new Uri(config.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSegundos);

      switch (config.TipoAutenticacao)
        {
            case "Bearer":
            if (!string.IsNullOrEmpty(config.Token))
     {
          _httpClient.DefaultRequestHeaders.Authorization =
         new AuthenticationHeaderValue("Bearer", config.Token);
                }
                break;

            case "ApiKey":
      if (!string.IsNullOrEmpty(config.Token))
    {
    _httpClient.DefaultRequestHeaders.Add("X-API-Key", config.Token);
                }
          break;

     case "Basic":
       if (!string.IsNullOrEmpty(config.ClientId) && !string.IsNullOrEmpty(config.ClientSecret))
  {
var credentials = Convert.ToBase64String(
        Encoding.UTF8.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
       _httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Basic", credentials);
       }
         break;
        }
    }

    /// <summary>
    /// Executar requisição HTTP com retry e logging
    /// </summary>
protected async Task<HttpResponseMessage?> ExecutarRequisicaoAsync(
        string apiExternaId,
     string endpoint,
    HttpMethod metodo,
        object? payload = null,
        string? usuarioId = null)
 {
        var config = await _context.Set<ApiExterna>().FindAsync(apiExternaId);
        if (config == null) return null;

        var stopwatch = Stopwatch.StartNew();
        HttpResponseMessage? response = null;
      Exception? ultimaExcecao = null;

        for (int tentativa = 1; tentativa <= config.MaxRetries; tentativa++)
        {
            try
            {
 HttpRequestMessage request = new(metodo, endpoint);

    if (payload != null && (metodo == HttpMethod.Post || metodo == HttpMethod.Put))
        {
            var json = JsonSerializer.Serialize(payload);
request.Content = new StringContent(json, Encoding.UTF8, "application/json");
          }

             response = await _httpClient.SendAsync(request);
     stopwatch.Stop();

  // Registrar log
             await RegistrarLogAsync(apiExternaId, endpoint, metodo.Method, response,
  payload, stopwatch.ElapsedMilliseconds, tentativa, usuarioId);

                if (response.IsSuccessStatusCode)
      {
  // Atualizar status da API
          await AtualizarStatusApiAsync(apiExternaId, true, null);
         return response;
   }

         // Se for erro 5xx, tentar novamente
     if ((int)response.StatusCode >= 500 && tentativa < config.MaxRetries)
     {
     await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, tentativa))); // Exponential backoff
      continue;
      }

         // Erro 4xx não deve fazer retry
          break;
            }
            catch (Exception ex)
            {
 ultimaExcecao = ex;
     _logger.LogError(ex, $"Erro na tentativa {tentativa} de {config.MaxRetries} para {_apiNome}/{endpoint}");

        if (tentativa < config.MaxRetries)
   {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, tentativa)));
             }
  }
    }

 stopwatch.Stop();

        // Registrar log de falha
      await RegistrarLogAsync(apiExternaId, endpoint, metodo.Method, response,
            payload, stopwatch.ElapsedMilliseconds, config.MaxRetries, usuarioId,
   ultimaExcecao?.Message);

        // Atualizar status da API com erro
 await AtualizarStatusApiAsync(apiExternaId, false, ultimaExcecao?.Message);

    return response;
  }

    /// <summary>
    /// Registrar log de integração
    /// </summary>
    private async Task RegistrarLogAsync(
        string apiExternaId,
        string endpoint,
        string metodoHttp,
        HttpResponseMessage? response,
        object? payload,
  long tempoMs,
        int numeroTentativa,
   string? usuarioId,
        string? mensagemErro = null)
  {
  try
   {
            var log = new LogIntegracaoApi
    {
              ApiExternaId = apiExternaId,
                Endpoint = endpoint,
                MetodoHttp = metodoHttp,
       StatusCode = response != null ? (int)response.StatusCode : null,
     Sucesso = response?.IsSuccessStatusCode ?? false,
   TempoRespostaMs = tempoMs,
        RequestPayload = payload != null ? JsonSerializer.Serialize(payload).Substring(0, Math.Min(2000, JsonSerializer.Serialize(payload).Length)) : null,
        ResponsePayload = response != null ? (await response.Content.ReadAsStringAsync()).Substring(0, Math.Min(2000, (await response.Content.ReadAsStringAsync()).Length)) : null,
                MensagemErro = mensagemErro,
             NumeroTentativa = numeroTentativa,
    UsuarioId = usuarioId,
      CreatedAt = DateTime.UtcNow
      };

    _context.Set<LogIntegracaoApi>().Add(log);
            await _context.SaveChangesAsync();
        }
 catch (Exception ex)
        {
         _logger.LogError(ex, "Erro ao registrar log de integração");
 }
    }

    /// <summary>
    /// Atualizar status da API após requisição
    /// </summary>
    private async Task AtualizarStatusApiAsync(string apiExternaId, bool sucesso, string? erro)
    {
        try
        {
   var api = await _context.Set<ApiExterna>().FindAsync(apiExternaId);
         if (api == null) return;

            api.UltimaTentativa = DateTime.UtcNow;

  if (sucesso)
 {
      api.UltimaSincronizacao = DateTime.UtcNow;
    api.TotalSincronizacoes++;
          api.Status = "ATIVA";
       api.UltimoErro = null;
            }
            else
  {
                api.TotalErros++;
                api.UltimoErro = erro?.Substring(0, Math.Min(2000, erro.Length));
        
          // Se muitos erros consecutivos, marcar como ERRO
     if (api.TotalErros > 10)
       {
        api.Status = "ERRO";
     }
     }

   api.UpdatedAt = DateTime.UtcNow;
 await _context.SaveChangesAsync();
        }
        catch (Exception ex)
      {
            _logger.LogError(ex, "Erro ao atualizar status da API");
        }
    }

    /// <summary>
    /// Deserializar resposta JSON
  /// </summary>
    protected async Task<T?> DeserializarRespostaAsync<T>(HttpResponseMessage response)
 {
     try
        {
         var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
      {
         PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
         _logger.LogError(ex, $"Erro ao deserializar resposta de {_apiNome}");
    return default;
        }
    }
}

/// <summary>
/// Serviço de integração com HORUS (Sistema Nacional de Gestão da Assistência Farmacêutica)
/// </summary>
public class HorusIntegrationService : ApiExternaServiceBase
{
    public HorusIntegrationService(
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext context,
        ILogger<HorusIntegrationService> logger)
    : base(httpClientFactory, context, logger, "HORUS")
    {
    }

    /// <summary>
    /// Sincronizar medicamentos do HORUS (alias)
    /// </summary>
    public Task<bool> SincronizarMedicamentosAsync(string? esfId = null, string? usuarioId = null)
    {
     return SincronizarEstoqueMedicamentosAsync(esfId, usuarioId);
    }

    /// <summary>
    /// Sincronizar estoque de medicamentos do HORUS
    /// </summary>
    public async Task<bool> SincronizarEstoqueMedicamentosAsync(string? esfId = null, string? usuarioId = null)
    {
        try
        {
  var config = await ObterConfiguracaoAsync(esfId);
            if (config == null)
      {
     _logger.LogWarning("Configuração do HORUS não encontrada");
     return false;
   }

         ConfigurarAutenticacao(config);

     // Endpoint exemplo: /api/v1/medicamentos/estoque
         var response = await ExecutarRequisicaoAsync(
  config.Id,
        "/api/v1/medicamentos/estoque",
  HttpMethod.Get,
         usuarioId: usuarioId);

    if (response == null || !response.IsSuccessStatusCode)
        {
     return false;
      }

            var medicamentos = await DeserializarRespostaAsync<List<HorusMedicamentoDto>>(response);
      if (medicamentos == null || !medicamentos.Any())
    {
            return true; // Sucesso, mas sem dados
    }

            // Salvar medicamentos no banco
            foreach (var med in medicamentos)
  {
        var existente = await _context.Set<HorusMedicamento>()
      .FirstOrDefaultAsync(m => m.CodigoHorus == med.Codigo && m.EsfId == esfId);

              if (existente != null)
                {
     // Atualizar
      existente.Nome = med.Nome;
     existente.PrincipioAtivo = med.PrincipioAtivo;
        existente.Concentracao = med.Concentracao;
    existente.FormaFarmaceutica = med.FormaFarmaceutica;
       existente.QuantidadeEstoque = med.Quantidade;
        existente.QuantidadeMinima = med.QuantidadeMinima;
        existente.CustoUnitario = med.CustoUnitario;
        existente.Lote = med.Lote;
        existente.DataValidade = med.DataValidade;
  existente.UltimaAtualizacaoHorus = DateTime.UtcNow;
           existente.UpdatedAt = DateTime.UtcNow;
          }
         else
         {
           // Criar novo
     var novoMed = new HorusMedicamento
  {
             CodigoHorus = med.Codigo,
         Nome = med.Nome,
          PrincipioAtivo = med.PrincipioAtivo,
             Concentracao = med.Concentracao,
    FormaFarmaceutica = med.FormaFarmaceutica,
          QuantidadeEstoque = med.Quantidade,
 QuantidadeMinima = med.QuantidadeMinima,
            CustoUnitario = med.CustoUnitario,
            Lote = med.Lote,
            DataValidade = med.DataValidade,
         EsfId = esfId,
  UltimaAtualizacaoHorus = DateTime.UtcNow
        };
         _context.Set<HorusMedicamento>().Add(novoMed);
                }
            }

     await _context.SaveChangesAsync();
            _logger.LogInformation($"Sincronizados {medicamentos.Count} medicamentos do HORUS");
     return true;
        }
   catch (Exception ex)
        {
       _logger.LogError(ex, "Erro ao sincronizar medicamentos do HORUS");
            return false;
        }
    }

    /// <summary>
    /// Obter medicamentos com estoque baixo
    /// </summary>
    public async Task<List<HorusMedicamento>> ObterMedicamentosEstoqueBaixoAsync(string? esfId = null)
    {
    var query = _context.Set<HorusMedicamento>()
      .Where(m => m.QuantidadeEstoque <= m.QuantidadeMinima);

    if (!string.IsNullOrEmpty(esfId))
        {
  query = query.Where(m => m.EsfId == esfId);
 }

        return await query.OrderBy(m => m.Nome).ToListAsync();
    }

    /// <summary>
    /// Obter todos os medicamentos de uma UBS
    /// </summary>
    public async Task<List<HorusMedicamento>> ObterMedicamentosPorUbsAsync(string esfId)
    {
        return await _context.Set<HorusMedicamento>()
            .Where(m => m.EsfId == esfId)
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    /// <summary>
    /// Obter custo total de medicamentos por UBS
    /// </summary>
    public async Task<decimal> ObterCustoTotalMedicamentosAsync(string esfId)
    {
        var medicamentos = await ObterMedicamentosPorUbsAsync(esfId);
        return medicamentos.Sum(m => m.CustoTotal);
    }

    /// <summary>
    /// Obter resumo de custos de medicamentos por UBS
    /// </summary>
    public async Task<HorusCustoResumoDto> ObterResumoCustosAsync(string esfId)
    {
        var medicamentos = await ObterMedicamentosPorUbsAsync(esfId);

        return new HorusCustoResumoDto
        {
            esfId = esfId,
            TotalMedicamentos = medicamentos.Count,
            QuantidadeTotal = medicamentos.Sum(m => m.QuantidadeEstoque),
            CustoTotal = medicamentos.Sum(m => m.CustoTotal),
            MedicamentosEstoqueBaixo = medicamentos.Count(m => m.QuantidadeEstoque <= m.QuantidadeMinima),
            CustoMedicamentosEstoqueBaixo = medicamentos
                .Where(m => m.QuantidadeEstoque <= m.QuantidadeMinima)
                .Sum(m => m.CustoTotal),
            UltimaSincronizacao = medicamentos.Max(m => m.UltimaAtualizacaoHorus)
        };
    }

    /// <summary>
    /// Obter custos de todas as UBS
    /// </summary>
    public async Task<List<HorusCustoResumoDto>> ObterCustosPorTodasUbsAsync()
    {
        var esfIds = await _context.Set<HorusMedicamento>()
            .Where(m => m.EsfId != null)
            .Select(m => m.EsfId!)
            .Distinct()
            .ToListAsync();

        var resumos = new List<HorusCustoResumoDto>();
        foreach (var esfId in esfIds)
        {
            var resumo = await ObterResumoCustosAsync(esfId);
            var ubs = await _context.Set<ESF>().FindAsync(esfId);
            if (ubs != null)
            {
                resumo.NomeUbs = ubs.Nome;
            }
            resumos.Add(resumo);
        }

        return resumos.OrderByDescending(r => r.CustoTotal).ToList();
    }
}

// DTOs para deserialização
public class HorusMedicamentoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? PrincipioAtivo { get; set; }
    public string? Concentracao { get; set; }
    public string? FormaFarmaceutica { get; set; }
    public int Quantidade { get; set; }
    public int QuantidadeMinima { get; set; }
    public decimal CustoUnitario { get; set; } = 0;
    public string? Lote { get; set; }
    public DateTime? DataValidade { get; set; }
}

public class HorusCustoResumoDto
{
    public string esfId { get; set; } = string.Empty;
    public string? NomeUbs { get; set; }
    public int TotalMedicamentos { get; set; }
    public int QuantidadeTotal { get; set; }
    public decimal CustoTotal { get; set; }
    public int MedicamentosEstoqueBaixo { get; set; }
    public decimal CustoMedicamentosEstoqueBaixo { get; set; }
    public DateTime? UltimaSincronizacao { get; set; }
}
