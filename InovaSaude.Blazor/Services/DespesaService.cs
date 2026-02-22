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
            .Include(d => d.Ubs)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Include(d => d.UsuarioAprovacao)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Despesa?> GetDespesaByIdAsync(string id)
    {
        return await _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Ubs)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .Include(d => d.UsuarioAprovacao)
            .Include(d => d.Anexos)
            .Include(d => d.HistoricoStatus)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Despesa>> GetDespesasByUBSAsync(string ubsId)
    {
        return await _context.Despesas
            .Where(d => d.UbsId == ubsId)
            .Include(d => d.Categoria)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Despesa>> GetDespesasByStatusAsync(string status)
    {
        return await _context.Despesas
            .Where(d => d.Status == status)
            .Include(d => d.Categoria)
            .Include(d => d.Ubs)
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
            .Include(d => d.Ubs)
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

            // Criar histórico inicial
            var historico = new HistoricoDespesa
            {
                DespesaId = despesa.Id,
                StatusNovo = despesa.Status,
                UsuarioId = despesa.UsuarioCriacaoId,
                Observacao = "Despesa criada"
            };
            _context.HistoricoDespesas.Add(historico);

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

    public async Task AprovarDespesaAsync(string despesaId, string usuarioId, string? observacao = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var despesa = await _context.Despesas.FindAsync(despesaId);
            if (despesa == null) throw new Exception("Despesa não encontrada");

            var statusAnterior = despesa.Status;
            despesa.Status = "APROVADA";
            despesa.UsuarioAprovacaoId = usuarioId;
            despesa.DataAprovacao = DateTime.UtcNow;

            _context.Despesas.Update(despesa);

            // Criar histórico
            var historico = new HistoricoDespesa
            {
                DespesaId = despesaId,
                StatusAnterior = statusAnterior,
                StatusNovo = "APROVADA",
                UsuarioId = usuarioId,
                Observacao = observacao ?? "Despesa aprovada"
            };
            _context.HistoricoDespesas.Add(historico);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RejeitarDespesaAsync(string despesaId, string usuarioId, string? observacao = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var despesa = await _context.Despesas.FindAsync(despesaId);
            if (despesa == null) throw new Exception("Despesa não encontrada");

            var statusAnterior = despesa.Status;
            despesa.Status = "REJEITADA";
            despesa.UsuarioAprovacaoId = usuarioId;
            despesa.DataAprovacao = DateTime.UtcNow;

            _context.Despesas.Update(despesa);

            // Criar histórico
            var historico = new HistoricoDespesa
            {
                DespesaId = despesaId,
                StatusAnterior = statusAnterior,
                StatusNovo = "REJEITADA",
                UsuarioId = usuarioId,
                Observacao = observacao ?? "Despesa rejeitada"
            };
            _context.HistoricoDespesas.Add(historico);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Despesa>> SearchDespesasAsync(string searchTerm, string? status = null, string? ubsId = null)
    {
        var query = _context.Despesas
            .Include(d => d.Categoria)
            .Include(d => d.Ubs)
            .Include(d => d.Fornecedor)
            .Include(d => d.UsuarioCriacao)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(d => d.Descricao.Contains(searchTerm) ||
                                    d.NumeroNota.Contains(searchTerm) ||
                                    d.NumeroEmpenho.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(d => d.Status == status);
        }

        if (!string.IsNullOrEmpty(ubsId))
        {
            query = query.Where(d => d.UbsId == ubsId);
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDespesasByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _context.Despesas
            .Where(d => d.CreatedAt >= inicio && d.CreatedAt <= fim && d.Status == "APROVADA")
            .SumAsync(d => d.Valor);
    }
}