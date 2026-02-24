using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class RelatorioService
{
    private readonly ApplicationDbContext _context;

    public RelatorioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RelatorioDespesas> GerarRelatorioDespesasAsync(
        DateTime dataInicio,
        DateTime dataFim,
        string? esfId = null,
        string? categoriaId = null)
    {
        var query = _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Where(d => d.CreatedAt >= dataInicio && d.CreatedAt <= dataFim)
            .AsQueryable();

        if (!string.IsNullOrEmpty(esfId))
            query = query.Where(d => d.EsfId == esfId);

        if (!string.IsNullOrEmpty(categoriaId))
            query = query.Where(d => d.CategoriaId == categoriaId);

        var despesas = await query.ToListAsync();

        var relatorio = new RelatorioDespesas
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            TotalDespesas = despesas.Sum(d => d.Valor),
            QuantidadeDespesas = despesas.Count,
            Despesas = despesas
        };

        // Agrupar por categoria
        relatorio.DespesasPorCategoria = despesas
            .GroupBy(d => d.Categoria.Nome)
            .Select(g => new RelatorioCategoria
            {
                Categoria = g.Key,
                ValorTotal = g.Sum(d => d.Valor),
                Quantidade = g.Count(),
                Percentual = 0 // Será calculado depois
            })
            .ToList();

        // Agrupar por ESF
        relatorio.DespesasPorESF = despesas
            .GroupBy(d => d.Esf.Nome)
            .Select(g => new RelatorioESF
            {
                ESF = g.Key,
                ValorTotal = g.Sum(d => d.Valor),
                Quantidade = g.Count(),
                Percentual = 0 // Será calculado depois
            })
            .ToList();

        // Calcular percentuais
        foreach (var item in relatorio.DespesasPorCategoria)
        {
            item.Percentual = relatorio.TotalDespesas > 0
                ? (item.ValorTotal / relatorio.TotalDespesas) * 100
                : 0;
        }

        foreach (var item in relatorio.DespesasPorESF)
        {
            item.Percentual = relatorio.TotalDespesas > 0
                ? (item.ValorTotal / relatorio.TotalDespesas) * 100
                : 0;
        }

        return relatorio;
    }

    public async Task<RelatorioESFDetalhado> GerarRelatorioESFAsync(string esfId, DateTime dataInicio, DateTime dataFim)
    {
        var esf = await _context.ESF
            .Include(e => e.Coordenador)
            .Include(e => e.Usuarios)
            .FirstOrDefaultAsync(e => e.Id == esfId);

        if (esf == null) throw new Exception("ESF não encontrada");

        var despesas = await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Where(d => d.EsfId == esfId &&
                       d.CreatedAt >= dataInicio &&
                       d.CreatedAt <= dataFim)
            .ToListAsync();

        return new RelatorioESFDetalhado
        {
            ESF = esf,
            DataInicio = dataInicio,
            DataFim = dataFim,
            TotalDespesas = despesas.Sum(d => d.Valor),
            QuantidadeDespesas = despesas.Count,
            Despesas = despesas,
            TotalUsuarios = esf.Usuarios.Count
        };
    }

    public async Task<List<RelatorioMensal>> GerarRelatorioMensalAsync(int ano)
    {
        var relatorios = new List<RelatorioMensal>();

        for (int mes = 1; mes <= 12; mes++)
        {
            var inicioMes = new DateTime(ano, mes, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            var despesas = await _context.Despesas
                .Where(d => d.CreatedAt >= inicioMes && d.CreatedAt <= fimMes)
                .ToListAsync();

            relatorios.Add(new RelatorioMensal
            {
                Ano = ano,
                Mes = mes,
                NomeMes = new DateTime(ano, mes, 1).ToString("MMMM"),
                TotalDespesas = despesas.Sum(d => d.Valor),
                QuantidadeDespesas = despesas.Count
            });
        }

        return relatorios;
    }

    public async Task<byte[]> ExportarRelatorioExcelAsync(RelatorioDespesas relatorio)
    {
        // Implementação simplificada - em produção usaria ClosedXML ou similar
        var csv = "Data,Descrição,Valor,Categoria,ESF\n";

        foreach (var despesa in relatorio.Despesas)
        {
            csv += $"{despesa.CreatedAt:yyyy-MM-dd},{despesa.Descricao},{despesa.Valor},{despesa.Categoria.Nome},{despesa.Esf.Nome}\n";
        }

        return System.Text.Encoding.UTF8.GetBytes(csv);
    }

    public async Task<byte[]> ExportarRelatorioPDFAsync(RelatorioDespesas relatorio)
    {
        // Implementação simplificada - em produção usaria iTextSharp ou similar
        var html = $@"
        <html>
        <body>
        <h1>Relatório de Despesas</h1>
        <p>Período: {relatorio.DataInicio:dd/MM/yyyy} - {relatorio.DataFim:dd/MM/yyyy}</p>
        <p>Total: R$ {relatorio.TotalDespesas:N2}</p>
        <p>Quantidade: {relatorio.QuantidadeDespesas}</p>
        </body>
        </html>";

        return System.Text.Encoding.UTF8.GetBytes(html);
    }
}

public class RelatorioDespesas
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
    public List<Despesa> Despesas { get; set; } = new();
    public List<RelatorioCategoria> DespesasPorCategoria { get; set; } = new();
    public List<RelatorioESF> DespesasPorESF { get; set; } = new();
}

public class RelatorioCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}

public class RelatorioESF
{
    public string ESF { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}

public class RelatorioESFDetalhado
{
    public ESF ESF { get; set; } = null!;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
    public List<Despesa> Despesas { get; set; } = new();
    public int TotalUsuarios { get; set; }
}

public class RelatorioMensal
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NomeMes { get; set; } = string.Empty;
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
}
