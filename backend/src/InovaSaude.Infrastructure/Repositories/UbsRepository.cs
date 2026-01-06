using InovaSaude.Core.Entities;
using InovaSaude.Core.Interfaces;
using InovaSaude.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Infrastructure.Repositories;

public class UbsRepository : Repository<UBS>, IUbsRepository
{
    public UbsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<UBS?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Municipio)
            .Include(u => u.Coordenadores)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<UBS>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Municipio)
            .ToListAsync();
    }

    public async Task<IEnumerable<UBS>> GetByMunicipioIdAsync(Guid municipioId)
    {
        return await _dbSet
            .Include(u => u.Municipio)
            .Where(u => u.MunicipioId == municipioId)
            .ToListAsync();
    }
}
