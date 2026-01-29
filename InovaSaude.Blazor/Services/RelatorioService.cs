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
        string? ubsId = null,
        string? categoriaId = null,
        string? status = null)
    {
        var query = _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Ubs)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Where(d => d.CreatedAt >= dataInicio && d.CreatedAt <= dataFim)
            .AsQueryable();

        if (!string.IsNullOrEmpty(ubsId))
            query = query.Where(d => d.UbsId == ubsId);

        if (!string.IsNullOrEmpty(categoriaId))
            query = query.Where(d => d.CategoriaId == categoriaId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(d => d.Status == status);

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

        // Agrupar por UBS
        relatorio.DespesasPorUBS = despesas
            .GroupBy(d => d.Ubs.Nome)
            .Select(g => new RelatorioUBS
            {
                UBS = g.Key,
                ValorTotal = g.Sum(d => d.Valor),
                Quantidade = g.Count(),
                Percentual = 0 // Será calculado depois
            })
            .ToList();

        // Agrupar por status
        relatorio.DespesasPorStatus = despesas
            .GroupBy(d => d.Status)
            .Select(g => new RelatorioStatus
            {
                Status = g.Key,
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

        foreach (var item in relatorio.DespesasPorUBS)
        {
            item.Percentual = relatorio.TotalDespesas > 0
                ? (item.ValorTotal / relatorio.TotalDespesas) * 100
                : 0;
        }

        foreach (var item in relatorio.DespesasPorStatus)
        {
            item.Percentual = relatorio.TotalDespesas > 0
                ? (item.ValorTotal / relatorio.TotalDespesas) * 100
                : 0;
        }

        return relatorio;
    }

    public async Task<RelatorioUBSDetalhado> GerarRelatorioUBSAsync(string ubsId, DateTime dataInicio, DateTime dataFim)
    {
        var ubs = await _context.UBS
            .Include(u => u.Coordenador)
            .Include(u => u.Usuarios)
            .FirstOrDefaultAsync(u => u.Id == ubsId);

        if (ubs == null) throw new Exception("UBS não encontrada");

        var despesas = await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Where(d => d.UbsId == ubsId &&
                       d.CreatedAt >= dataInicio &&
                       d.CreatedAt <= dataFim)
            .ToListAsync();

        return new RelatorioUBSDetalhado
        {
            UBS = ubs,
            DataInicio = dataInicio,
            DataFim = dataFim,
            TotalDespesas = despesas.Sum(d => d.Valor),
            QuantidadeDespesas = despesas.Count,
            DespesasAprovadas = despesas.Count(d => d.Status == "APROVADA"),
            DespesasPendentes = despesas.Count(d => d.Status == "PENDENTE"),
            DespesasRejeitadas = despesas.Count(d => d.Status == "REJEITADA"),
            Despesas = despesas,
            TotalUsuarios = ubs.Usuarios.Count
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
                QuantidadeDespesas = despesas.Count,
                DespesasAprovadas = despesas.Count(d => d.Status == "APROVADA"),
                DespesasPendentes = despesas.Count(d => d.Status == "PENDENTE")
            });
        }

        return relatorios;
    }

    public async Task<byte[]> ExportarRelatorioExcelAsync(RelatorioDespesas relatorio)
    {
        // Implementação simplificada - em produção usaria ClosedXML ou similar
        var csv = "Data,Descrição,Valor,Categoria,UBS,Status\n";

        foreach (var despesa in relatorio.Despesas)
        {
            csv += $"{despesa.CreatedAt:yyyy-MM-dd},{despesa.Descricao},{despesa.Valor},{despesa.Categoria.Nome},{despesa.Ubs.Nome},{despesa.Status}\n";
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
    public List<RelatorioUBS> DespesasPorUBS { get; set; } = new();
    public List<RelatorioStatus> DespesasPorStatus { get; set; } = new();
}

public class RelatorioCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}

public class RelatorioUBS
{
    public string UBS { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}

public class RelatorioStatus
{
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int Quantidade { get; set; }
    public decimal Percentual { get; set; }
}

public class RelatorioUBSDetalhado
{
    public UBS UBS { get; set; } = null!;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
    public int DespesasAprovadas { get; set; }
    public int DespesasPendentes { get; set; }
    public int DespesasRejeitadas { get; set; }
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
    public int DespesasAprovadas { get; set; }
    public int DespesasPendentes { get; set; }
}