using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var stats = new DashboardStats();

        // Total de ESF ativas
        stats.TotalESF = await _context.ESF
            .AsNoTracking()
            .CountAsync(e => e.Status == "ATIVA");

        // Total de despesas no mês atual
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        stats.TotalDespesasMes = await _context.Despesas
            .AsNoTracking()
            .Where(d => d.CreatedAt.Month == currentMonth && d.CreatedAt.Year == currentYear)
            .SumAsync(d => d.Valor);

        // Total de usuários ativos
        stats.TotalUsuarios = await _context.Usuarios
            .AsNoTracking()
            .CountAsync(u => u.Status == "ATIVO");

        // Despesas por categoria (últimos 6 meses)
        stats.DespesasPorCategoria = await _context.Despesas
            .AsNoTracking()
            .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(d => d.Categoria.Nome)
            .Select(g => new CategoriaStats
            {
                Nome = g.Key,
                Valor = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .ToListAsync();

        // Despesas por ESF (últimos 6 meses)
        stats.DespesasPorESF = await _context.Despesas
            .AsNoTracking()
            .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(d => d.Esf.Nome)
            .Select(g => new ESFStats
            {
                Nome = g.Key,
                Valor = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .ToListAsync();

        // Últimas atividades
        stats.UltimasAtividades = await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Select(a => new AtividadeRecente
            {
                Descricao = $"{a.Action} - {a.EntityType}",
                Usuario = a.UserName ?? "Sistema",
                DataHora = a.CreatedAt
            })
            .ToListAsync();

        return stats;
    }

    public async Task<List<Despesa>> GetDespesasRecentesAsync(int limit = 10)
    {
        return await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .OrderByDescending(d => d.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<SystemEvent>> GetAlertasSistemaAsync()
    {
        return await _context.SystemEvents
            .AsNoTracking()
            .Where(e => !e.Acknowledged)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .ToListAsync();
    }

    public async Task<DashboardResumoMensal> GetDashboardResumoMensalAsync(DateTime inicio, DateTime fim)
    {
        var despesasQuery = _context.Despesas
            .AsNoTracking()
            .Where(d => d.MesReferencia >= inicio && d.MesReferencia <= fim);

        var totalDespesas = await despesasQuery.SumAsync(d => d.Valor);
        var quantidadeDespesas = await despesasQuery.CountAsync();

        var topCategorias = await despesasQuery
            .GroupBy(d => d.Categoria != null ? d.Categoria.Nome : "Sem Categoria")
            .Select(g => new CategoriaData2
            {
                Categoria = g.Key,
                Total = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .Take(5)
            .ToListAsync();

        var topEsf = await despesasQuery
            .GroupBy(d => d.Esf != null ? d.Esf.Nome : "Sem ESF")
            .Select(g => new ESFResumoData
            {
                Nome = g.Key,
                Total = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .OrderByDescending(e => e.Total)
            .Take(5)
            .ToListAsync();

        var funcionarios = await _context.Funcionarios.AsNoTracking().ToListAsync();

        return new DashboardResumoMensal
        {
            TotalDespesas = totalDespesas,
            QuantidadeDespesas = quantidadeDespesas,
            TotalSalarios = funcionarios.Sum(f => f.Salario),
            QuantidadeFuncionarios = funcionarios.Count,
            QuantidadeESF = await _context.ESF.AsNoTracking().CountAsync(),
            TopCategorias = topCategorias,
            TopEsf = topEsf
        };
    }

    public async Task<DashboardData> GetDashboardDataAsync(string? esfId = null, DateTime? inicio = null, DateTime? fim = null)
    {
        inicio ??= DateTime.UtcNow.AddMonths(-1);
        fim ??= DateTime.UtcNow;

        var query = _context.Despesas
            .AsNoTracking()
            .Where(d => d.CreatedAt >= inicio && d.CreatedAt <= fim);

        if (!string.IsNullOrEmpty(esfId))
        {
            query = query.Where(d => d.EsfId == esfId);
        }

        var porCategoria = await query
            .GroupBy(d => d.Categoria != null ? d.Categoria.Nome : "Sem Categoria")
            .Select(g => new CategoriaData2
            {
                Categoria = g.Key,
                Total = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .ToListAsync();

        return new DashboardData
        {
            TotalGeral = await query.SumAsync(d => d.Valor),
            TotalDespesas = await query.CountAsync(),
            DespesasPorCategoria = porCategoria
        };
    }
}

public class DashboardStats
{
    public int TotalESF { get; set; }
    public decimal TotalDespesasMes { get; set; }
    public int TotalUsuarios { get; set; }
    public List<CategoriaStats> DespesasPorCategoria { get; set; } = new();
    public List<ESFStats> DespesasPorESF { get; set; } = new();
    public List<AtividadeRecente> UltimasAtividades { get; set; } = new();
}

public class CategoriaStats
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
}

public class ESFStats
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
}

public class AtividadeRecente
{
    public string Descricao { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
}

public class DashboardData
{
    public decimal TotalGeral { get; set; }
    public int TotalDespesas { get; set; }
    public List<CategoriaData2> DespesasPorCategoria { get; set; } = new();
}

public class CategoriaData2
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Quantidade { get; set; }
}

public class ESFResumoData
{
    public string Nome { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Quantidade { get; set; }
}

public class DashboardResumoMensal
{
    public decimal TotalDespesas { get; set; }
    public int QuantidadeDespesas { get; set; }
    public decimal TotalSalarios { get; set; }
    public int QuantidadeFuncionarios { get; set; }
    public int QuantidadeESF { get; set; }
    public List<CategoriaData2> TopCategorias { get; set; } = new();
    public List<ESFResumoData> TopEsf { get; set; } = new();
}
