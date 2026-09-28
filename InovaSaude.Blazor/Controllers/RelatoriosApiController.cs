using InovaSaude.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InovaSaude.Blazor.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
[Authorize]
public class RelatoriosApiController : ControllerBase
{
    private readonly RelatorioService _relatorioService;

    public RelatoriosApiController(RelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    [HttpGet("mensal/{ano:int}")]
    [EnableRateLimiting("api-read")]
    public async Task<IActionResult> GetRelatorioMensal([FromRoute] int ano)
    {
        if (ano < 2000 || ano > 2100)
        {
            return BadRequest(new { message = "Ano inválido." });
        }

        var relatorios = await _relatorioService.GerarRelatorioMensalAsync(ano);

        return Ok(relatorios.Select(r => new
        {
            r.Ano,
            r.Mes,
            r.NomeMes,
            r.TotalDespesas,
            r.QuantidadeDespesas
        }));
    }

    [HttpGet("despesas-resumo")]
    [EnableRateLimiting("api-read")]
    public async Task<IActionResult> GetRelatorioDespesasResumo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] string? esfId = null,
        [FromQuery] string? categoriaId = null)
    {
        if (dataInicio == default || dataFim == default)
        {
            return BadRequest(new { message = "Informe dataInicio e dataFim." });
        }

        if (dataFim < dataInicio)
        {
            return BadRequest(new { message = "dataFim deve ser maior ou igual a dataInicio." });
        }

        if ((dataFim - dataInicio).TotalDays > 366)
        {
            return BadRequest(new { message = "Período máximo permitido é de 366 dias." });
        }

        var relatorio = await _relatorioService.GerarRelatorioDespesasAsync(dataInicio, dataFim, esfId, categoriaId);

        return Ok(new
        {
            relatorio.DataInicio,
            relatorio.DataFim,
            relatorio.TotalDespesas,
            relatorio.QuantidadeDespesas,
            DespesasPorCategoria = relatorio.DespesasPorCategoria.Select(x => new
            {
                x.Categoria,
                x.ValorTotal,
                x.Quantidade,
                x.Percentual
            }),
            DespesasPorESF = relatorio.DespesasPorESF.Select(x => new
            {
                x.ESF,
                x.ValorTotal,
                x.Quantidade,
                x.Percentual
            })
        });
    }
}
