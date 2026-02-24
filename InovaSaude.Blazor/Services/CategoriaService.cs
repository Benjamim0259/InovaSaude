using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class CategoriaService
{
    private readonly ApplicationDbContext _context;

    public CategoriaService(ApplicationDbContext context)
    {
     _context = context;
    }

    public async Task<List<Categoria>> GetAllCategoriasAsync()
    {
        return await _context.Categorias
   .OrderBy(c => c.Nome)
  .ToListAsync();
  }

public async Task<Categoria?> GetCategoriaByIdAsync(string id)
    {
        return await _context.Categorias
            .Include(c => c.Despesas)
        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Categoria>> GetCategoriasByTipoAsync(string tipo)
    {
        return await _context.Categorias
            .Where(c => c.Tipo == tipo)
         .OrderBy(c => c.Nome)
.ToListAsync();
    }

    public async Task CreateCategoriaAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoriaAsync(Categoria categoria)
    {
        categoria.UpdatedAt = DateTime.UtcNow;
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoriaAsync(string id)
    {
     var categoria = await _context.Categorias.FindAsync(id);
   if (categoria != null)
        {
            _context.Categorias.Remove(categoria);
     await _context.SaveChangesAsync();
  }
    }

    public async Task<decimal> GetTotalGastoPorCategoriaAsync(string categoriaId, DateTime inicio, DateTime fim)
    {
        return await _context.Despesas
       .Where(d => d.CategoriaId == categoriaId &&
           d.CreatedAt >= inicio &&
     d.CreatedAt <= fim &&
              true)
            .SumAsync(d => d.Valor);
    }

    public async Task<List<Categoria>> GetCategoriasComOrcamentoAsync()
    {
    return await _context.Categorias
            .Where(c => c.OrcamentoMensal.HasValue && c.OrcamentoMensal > 0)
        .OrderBy(c => c.Nome)
            .ToListAsync();
  }
}
