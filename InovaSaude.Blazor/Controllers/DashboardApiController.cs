using InovaSaude.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;

namespace InovaSaude.Blazor.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardApiController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardApiController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    [EnableRateLimiting("api-read")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _dashboardService.GetDashboardStatsAsync();

        return Ok(new
        {
            stats.TotalESF,
            stats.TotalDespesasMes,
            stats.TotalUsuarios,
            DespesasPorCategoria = stats.DespesasPorCategoria.Select(x => new { x.Nome, x.Valor, x.Quantidade }),
            DespesasPorESF = stats.DespesasPorESF.Select(x => new { x.Nome, x.Valor, x.Quantidade }),
            UltimasAtividades = stats.UltimasAtividades.Select(x => new { x.Descricao, x.Usuario, x.DataHora })
        });
    }

    [HttpGet("resumo-mensal")]
    [EnableRateLimiting("api-read")]
    public async Task<IActionResult> GetResumoMensal([FromQuery] string? mes = null)
    {
        DateTime referencia;
        if (string.IsNullOrWhiteSpace(mes))
        {
            var now = DateTime.UtcNow;
            referencia = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        else if (!DateTime.TryParseExact(mes + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return BadRequest(new { message = "Parâmetro 'mes' inválido. Use o formato yyyy-MM." });
        }
        else
        {
            referencia = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
        }

        var inicio = referencia;
        var fim = referencia.AddMonths(1).AddSeconds(-1);
        var resumo = await _dashboardService.GetDashboardResumoMensalAsync(inicio, fim);

        return Ok(new
        {
            Mes = referencia.ToString("yyyy-MM"),
            resumo.TotalDespesas,
            resumo.QuantidadeDespesas,
            resumo.TotalSalarios,
            resumo.QuantidadeFuncionarios,
            resumo.QuantidadeESF,
            TopCategorias = resumo.TopCategorias.Select(x => new { x.Categoria, x.Total, x.Quantidade }),
            TopEsf = resumo.TopEsf.Select(x => new { x.Nome, x.Total, x.Quantidade })
        });
    }
}
