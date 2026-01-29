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

        // Total de UBS ativas
        stats.TotalUBS = await _context.UBS.CountAsync(u => u.Status == "ATIVA");

        // Total de despesas no mês atual
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        stats.TotalDespesasMes = await _context.Despesas
            .Where(d => d.CreatedAt.Month == currentMonth && d.CreatedAt.Year == currentYear)
            .SumAsync(d => d.Valor);

        // Despesas pendentes
        stats.DespesasPendentes = await _context.Despesas
            .CountAsync(d => d.Status == "PENDENTE");

        // Total de usuários ativos
        stats.TotalUsuarios = await _context.Usuarios
            .CountAsync(u => u.Status == "ATIVO");

        // Despesas por categoria (últimos 6 meses)
        stats.DespesasPorCategoria = await _context.Despesas
            .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(d => d.Categoria.Nome)
            .Select(g => new CategoriaStats
            {
                Nome = g.Key,
                Valor = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .ToListAsync();

        // Despesas por UBS (últimos 6 meses)
        stats.DespesasPorUBS = await _context.Despesas
            .Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(d => d.Ubs.Nome)
            .Select(g => new UBSStats
            {
                Nome = g.Key,
                Valor = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .ToListAsync();

        // Últimas atividades
        stats.UltimasAtividades = await _context.AuditLogs
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
            .Include(d => d.Ubs)
            .Include(d => d.Fornecedor)
            .OrderByDescending(d => d.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<SystemEvent>> GetAlertasSistemaAsync()
    {
        return await _context.SystemEvents
            .Where(e => !e.Acknowledged)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}

public class DashboardStats
{
    public int TotalUBS { get; set; }
    public decimal TotalDespesasMes { get; set; }
    public int DespesasPendentes { get; set; }
    public int TotalUsuarios { get; set; }
    public List<CategoriaStats> DespesasPorCategoria { get; set; } = new();
    public List<UBSStats> DespesasPorUBS { get; set; } = new();
    public List<AtividadeRecente> UltimasAtividades { get; set; } = new();
}

public class CategoriaStats
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
}

public class UBSStats
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