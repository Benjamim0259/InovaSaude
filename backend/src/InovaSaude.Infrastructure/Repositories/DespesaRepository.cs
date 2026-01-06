using InovaSaude.Core.Entities;
using InovaSaude.Core.Interfaces;
using InovaSaude.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Infrastructure.Repositories;

public class DespesaRepository : Repository<Despesa>, IDespesaRepository
{
    public DespesaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Despesa?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(d => d.Ubs)
            .Include(d => d.Categoria)
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public override async Task<IEnumerable<Despesa>> GetAllAsync()
    {
        return await _dbSet
            .Include(d => d.Ubs)
            .Include(d => d.Categoria)
            .Include(d => d.Usuario)
            .ToListAsync();
    }

    public async Task<IEnumerable<Despesa>> GetByUbsIdAsync(Guid ubsId)
    {
        return await _dbSet
            .Include(d => d.Categoria)
            .Include(d => d.Usuario)
            .Where(d => d.UbsId == ubsId)
            .OrderByDescending(d => d.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<Despesa>> GetByPeriodAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(d => d.Ubs)
            .Include(d => d.Categoria)
            .Include(d => d.Usuario)
            .Where(d => d.Data >= startDate && d.Data <= endDate)
            .OrderByDescending(d => d.Data)
            .ToListAsync();
    }

    public async Task<IEnumerable<Despesa>> GetByUbsAndPeriodAsync(Guid ubsId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(d => d.Categoria)
            .Include(d => d.Usuario)
            .Where(d => d.UbsId == ubsId && d.Data >= startDate && d.Data <= endDate)
            .OrderByDescending(d => d.Data)
            .ToListAsync();
    }
}
