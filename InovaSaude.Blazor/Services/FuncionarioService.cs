using Microsoft.EntityFrameworkCore;
using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;

namespace InovaSaude.Blazor.Services;

public class FuncionarioService
{
    private readonly ApplicationDbContext _context;

    public FuncionarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Funcionario>> GetAllAsync()
    {
        return await _context.Funcionarios
            .Include(f => f.Esf)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<List<Funcionario>> GetByEsfIdAsync(string esfId)
    {
        return await _context.Funcionarios
            .Include(f => f.Esf)
            .Where(f => f.EsfId == esfId)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<Funcionario?> GetByIdAsync(string id)
    {
        return await _context.Funcionarios
            .Include(f => f.Esf)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Funcionario> CreateAsync(Funcionario funcionario)
    {
        funcionario.CreatedAt = DateTime.UtcNow;
        funcionario.UpdatedAt = DateTime.UtcNow;
        _context.Funcionarios.Add(funcionario);
        await _context.SaveChangesAsync();
        return funcionario;
    }

    public async Task<Funcionario> UpdateAsync(Funcionario funcionario)
    {
        funcionario.UpdatedAt = DateTime.UtcNow;
        _context.Funcionarios.Update(funcionario);
        await _context.SaveChangesAsync();
        return funcionario;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null) return false;

        _context.Funcionarios.Remove(funcionario);
        await _context.SaveChangesAsync();
        return true;
    }
}
