using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class UBSService
{
    private readonly ApplicationDbContext _context;

    public UBSService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UBS>> GetAllUBSAsync()
    {
        return await _context.UBS
            .Include(u => u.Coordenador)
            .Include(u => u.Usuarios)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<UBS?> GetUBSByIdAsync(string id)
    {
        return await _context.UBS
            .Include(u => u.Coordenador)
            .Include(u => u.Usuarios)
            .Include(u => u.Despesas)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UBS?> GetUBSByCodigoAsync(string codigo)
    {
        return await _context.UBS
            .Include(u => u.Coordenador)
            .Include(u => u.Usuarios)
            .FirstOrDefaultAsync(u => u.Codigo == codigo);
    }

    public async Task CreateUBSAsync(UBS ubs)
    {
        _context.UBS.Add(ubs);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUBSAsync(UBS ubs)
    {
        _context.UBS.Update(ubs);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUBSAsync(string id)
    {
        var ubs = await _context.UBS.FindAsync(id);
        if (ubs != null)
        {
            _context.UBS.Remove(ubs);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UBS>> GetUBSAtivasAsync()
    {
        return await _context.UBS
            .Where(u => u.Status == "ATIVA")
            .Include(u => u.Coordenador)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<List<Usuario>> GetCoordenadoresDisponiveisAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Perfil == PerfilUsuario.COORDENADOR && u.Status == "ATIVO")
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<dynamic> GetUBSStatsAsync(string ubsId)
    {
        // Total de usuários
        var totalUsuarios = await _context.Usuarios
            .CountAsync(u => u.UbsId == ubsId && u.Status == "ATIVO");

        // Total de despesas no mês atual
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var totalDespesasMes = await _context.Despesas
            .Where(d => d.UbsId == ubsId &&
                       d.CreatedAt.Month == currentMonth &&
                       d.CreatedAt.Year == currentYear)
            .SumAsync(d => d.Valor);

        // Despesas pendentes
        var despesasPendentes = await _context.Despesas
            .CountAsync(d => d.UbsId == ubsId && d.Status == "PENDENTE");

        // Despesas por categoria
        var despesasPorCategoria = await _context.Despesas
            .Where(d => d.UbsId == ubsId && d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
            .GroupBy(d => d.Categoria.Nome)
            .Select(g => new
            {
                Nome = g.Key,
                Valor = g.Sum(d => d.Valor),
                Quantidade = g.Count()
            })
            .ToListAsync();

        return new
        {
            TotalUsuarios = totalUsuarios,
            TotalDespesasMes = totalDespesasMes,
            DespesasPendentes = despesasPendentes,
            DespesasPorCategoria = despesasPorCategoria
        };
    }

    public async Task<List<UBS>> SearchUBSAsync(string searchTerm)
    {
        return await _context.UBS
            .Where(u => u.Nome.Contains(searchTerm) ||
                       u.Codigo.Contains(searchTerm) ||
                       u.Bairro.Contains(searchTerm) ||
                       (u.Endereco != null && u.Endereco.Contains(searchTerm)))
            .Include(u => u.Coordenador)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }
}