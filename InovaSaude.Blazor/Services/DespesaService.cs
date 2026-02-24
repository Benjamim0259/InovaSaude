using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class DespesaService
{
    private readonly ApplicationDbContext _context;

    public DespesaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Despesa>> GetAllDespesasAsync()
    {
        return await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Despesa?> GetDespesaByIdAsync(string id)
    {
        return await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Include(d => d.Anexos)
            .Include(d => d.HistoricoStatus)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Despesa>> GetDespesasByESFAsync(string esfId)
    {
        return await _context.Despesas
            .Where(d => d.EsfId == esfId)
            .Include(d => d.Categoria)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Despesa>> GetDespesasByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _context.Despesas
            .Where(d => d.CreatedAt >= inicio && d.CreatedAt <= fim)
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateDespesaAsync(Despesa despesa)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Validar se o usuário existe
            var usuarioExists = await _context.Usuarios.AnyAsync(u => u.Id == despesa.UsuarioCriacaoId);
            if (!usuarioExists)
            {
                throw new Exception("Usuário de criação não encontrado");
            }

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateDespesaAsync(Despesa despesa)
    {
        _context.Despesas.Update(despesa);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDespesaAsync(string id)
    {
        var despesa = await _context.Despesas.FindAsync(id);
        if (despesa != null)
        {
            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Despesa>> SearchDespesasAsync(string searchTerm, string? esfId = null)
    {
        var query = _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Esf)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(d => d.Descricao.Contains(searchTerm) ||
                                    (d.NumeroNota != null && d.NumeroNota.Contains(searchTerm)) ||
                                    (d.NumeroEmpenho != null && d.NumeroEmpenho.Contains(searchTerm)));
        }

        if (!string.IsNullOrEmpty(esfId))
        {
            query = query.Where(d => d.EsfId == esfId);
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDespesasByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _context.Despesas
            .Where(d => d.CreatedAt >= inicio && d.CreatedAt <= fim)
            .SumAsync(d => d.Valor);
    }
}