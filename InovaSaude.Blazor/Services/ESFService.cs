using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class ESFService
{
    private readonly ApplicationDbContext _context;

    public ESFService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ESF>> GetAllESFAsync()
    {
        return await _context.ESF
            .Include(e => e.Coordenador)
            .Include(e => e.Usuarios)
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }

    public async Task<ESF?> GetESFByIdAsync(string id)
    {
        return await _context.ESF
            .Include(e => e.Coordenador)
            .Include(e => e.Usuarios)
            .Include(e => e.Despesas)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ESF?> GetESFByCodigoAsync(string codigo)
    {
        return await _context.ESF
            .Include(e => e.Coordenador)
            .Include(e => e.Usuarios)
            .FirstOrDefaultAsync(e => e.Codigo == codigo);
    }

    public async Task CreateESFAsync(ESF esf)
    {
        _context.ESF.Add(esf);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateESFAsync(ESF esf)
    {
        _context.ESF.Update(esf);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteESFAsync(string id)
    {
        var esf = await _context.ESF.FindAsync(id);
        if (esf != null)
        {
            _context.ESF.Remove(esf);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<ESF>> GetESFAtivasAsync()
    {
        return await _context.ESF
            .Where(e => e.Status == "ATIVA")
            .Include(e => e.Coordenador)
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }

    public async Task<List<Usuario>> GetCoordenadoresDisponiveisAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Perfil == PerfilUsuario.COORDENADOR && u.Status == "ATIVO")
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<dynamic> GetESFStatsAsync(string esfId)
    {
        // Total de usuários
        var totalUsuarios = await _context.Usuarios
            .CountAsync(u => u.EsfId == esfId && u.Status == "ATIVO");

        // Total de despesas no mês atual
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var totalDespesasMes = await _context.Despesas
            .Where(d => d.EsfId == esfId &&
                       d.CreatedAt.Month == currentMonth &&
                       d.CreatedAt.Year == currentYear)
            .SumAsync(d => d.Valor);

        // Despesas por categoria
        var despesasPorCategoria = await _context.Despesas
            .Where(d => d.EsfId == esfId && d.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
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
            DespesasPorCategoria = despesasPorCategoria
        };
    }

    public async Task<List<ESF>> SearchESFAsync(string searchTerm)
    {
        return await _context.ESF
            .Where(e => e.Nome.Contains(searchTerm) ||
                       e.Codigo.Contains(searchTerm) ||
                       e.Bairro.Contains(searchTerm) ||
                       (e.Endereco != null && e.Endereco.Contains(searchTerm)))
            .Include(e => e.Coordenador)
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }
}
