using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class EstoqueFarmaciaService
{
    private readonly ApplicationDbContext _context;

    public EstoqueFarmaciaService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
  /// Obter todo o estoque
    /// </summary>
    public async Task<List<EstoqueFarmacia>> ObterTodoEstoqueAsync()
    {
        return await _context.Set<EstoqueFarmacia>()
 .Include(e => e.Movimentacoes)
  .OrderBy(e => e.NomeMedicamento)
            .ToListAsync();
    }

    /// <summary>
    /// Obter medicamentos com estoque baixo
    /// </summary>
    public async Task<List<EstoqueFarmacia>> ObterEstoqueBaixoAsync()
    {
        return await _context.Set<EstoqueFarmacia>()
            .Where(e => e.QuantidadeAtual <= e.QuantidadeMinima)
      .OrderBy(e => e.QuantidadeAtual)
            .ToListAsync();
    }

    /// <summary>
    /// Obter medicamentos próximos ao vencimento
    /// </summary>
    public async Task<List<EstoqueFarmacia>> ObterProximosVencimentoAsync(int diasAlerta = 90)
  {
        var dataLimite = DateTime.UtcNow.AddDays(diasAlerta);

   return await _context.Set<EstoqueFarmacia>()
            .Where(e => e.DataValidade.HasValue && e.DataValidade.Value <= dataLimite)
     .OrderBy(e => e.DataValidade)
      .ToListAsync();
    }

    /// <summary>
    /// Adicionar medicamento ao estoque (entrada)
    /// </summary>
    public async Task<EstoqueFarmacia> AdicionarEstoqueAsync(
      string nomeMedicamento,
        int quantidade,
        string usuarioId,
        string? principioAtivo = null,
        string? concentracao = null,
        string? formaFarmaceutica = null,
        string? lote = null,
        DateTime? dataValidade = null,
     string? localizacao = null,
        int quantidadeMinima = 0,
    string? numeroDocumento = null,
  string? motivo = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Verificar se já existe
          var existente = await _context.Set<EstoqueFarmacia>()
         .FirstOrDefaultAsync(e => e.NomeMedicamento == nomeMedicamento &&
     e.Lote == lote);

            EstoqueFarmacia estoque;

   if (existente != null)
         {
    // Atualizar estoque existente
 var qtdAnterior = existente.QuantidadeAtual;
       existente.QuantidadeAtual += quantidade;
           existente.UltimaMovimentacao = DateTime.UtcNow;
  existente.UpdatedAt = DateTime.UtcNow;

  // Atualizar status
     if (existente.QuantidadeAtual > existente.QuantidadeMinima)
    {
 existente.Status = "DISPONIVEL";
        }

        // Criar movimentação
       var movimentacao = new MovimentacaoEstoque
                {
     EstoqueFarmaciaId = existente.Id,
Tipo = "ENTRADA",
         Quantidade = quantidade,
                 QuantidadeAnterior = qtdAnterior,
               QuantidadeApos = existente.QuantidadeAtual,
        Motivo = motivo ?? "Entrada de medicamento",
     NumeroDocumento = numeroDocumento,
             UsuarioId = usuarioId
        };

     _context.Set<MovimentacaoEstoque>().Add(movimentacao);
         estoque = existente;
    }
        else
  {
           // Criar novo item no estoque
   estoque = new EstoqueFarmacia
     {
       NomeMedicamento = nomeMedicamento,
  PrincipioAtivo = principioAtivo,
               Concentracao = concentracao,
             FormaFarmaceutica = formaFarmaceutica,
      Lote = lote,
     DataValidade = dataValidade,
  QuantidadeAtual = quantidade,
       QuantidadeMinima = quantidadeMinima,
           Localizacao = localizacao,
        Status = "DISPONIVEL",
      UltimaMovimentacao = DateTime.UtcNow
  };

       _context.Set<EstoqueFarmacia>().Add(estoque);
           await _context.SaveChangesAsync(); // Para gerar o ID

         // Criar movimentação
         var movimentacao = new MovimentacaoEstoque
             {
        EstoqueFarmaciaId = estoque.Id,
          Tipo = "ENTRADA",
     Quantidade = quantidade,
             QuantidadeAnterior = 0,
       QuantidadeApos = quantidade,
       Motivo = motivo ?? "Entrada inicial de medicamento",
           NumeroDocumento = numeroDocumento,
                    UsuarioId = usuarioId
         };

         _context.Set<MovimentacaoEstoque>().Add(movimentacao);
            }

 await _context.SaveChangesAsync();
        await transaction.CommitAsync();

  return estoque;
        }
     catch
  {
            await transaction.RollbackAsync();
 throw;
     }
    }

    /// <summary>
    /// Fazer ajuste de estoque
    /// </summary>
    public async Task<bool> AjustarEstoqueAsync(
   string estoqueId,
     int novaQuantidade,
 string usuarioId,
        string motivo)
    {
 using var transaction = await _context.Database.BeginTransactionAsync();

        try
 {
          var estoque = await _context.Set<EstoqueFarmacia>().FindAsync(estoqueId);
            if (estoque == null) return false;

var qtdAnterior = estoque.QuantidadeAtual;
            var diferenca = novaQuantidade - qtdAnterior;

            estoque.QuantidadeAtual = novaQuantidade;
      estoque.UltimaMovimentacao = DateTime.UtcNow;
            estoque.UpdatedAt = DateTime.UtcNow;

            // Atualizar status
 if (novaQuantidade <= estoque.QuantidadeMinima)
            {
           estoque.Status = "BAIXO_ESTOQUE";
            }
  else if (novaQuantidade > estoque.QuantidadeMinima)
       {
                estoque.Status = "DISPONIVEL";
            }

       // Criar movimentação
            var movimentacao = new MovimentacaoEstoque
            {
        EstoqueFarmaciaId = estoqueId,
       Tipo = "AJUSTE",
          Quantidade = diferenca,
          QuantidadeAnterior = qtdAnterior,
        QuantidadeApos = novaQuantidade,
 Motivo = motivo,
    UsuarioId = usuarioId
       };

            _context.Set<MovimentacaoEstoque>().Add(movimentacao);

    await _context.SaveChangesAsync();
       await transaction.CommitAsync();

   return true;
        }
        catch
        {
            await transaction.RollbackAsync();
throw;
        }
    }

    /// <summary>
    /// Registrar perda de medicamento
    /// </summary>
    public async Task<bool> RegistrarPerdaAsync(
 string estoqueId,
    int quantidade,
        string usuarioId,
        string motivo)
 {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
  var estoque = await _context.Set<EstoqueFarmacia>().FindAsync(estoqueId);
     if (estoque == null || estoque.QuantidadeAtual < quantidade)
          return false;

         var qtdAnterior = estoque.QuantidadeAtual;
  estoque.QuantidadeAtual -= quantidade;
  estoque.UltimaMovimentacao = DateTime.UtcNow;
    estoque.UpdatedAt = DateTime.UtcNow;

       // Atualizar status
            if (estoque.QuantidadeAtual <= estoque.QuantidadeMinima)
        {
      estoque.Status = "BAIXO_ESTOQUE";
}

       // Criar movimentação
     var movimentacao = new MovimentacaoEstoque
            {
             EstoqueFarmaciaId = estoqueId,
     Tipo = "PERDA",
                Quantidade = -quantidade,
    QuantidadeAnterior = qtdAnterior,
       QuantidadeApos = estoque.QuantidadeAtual,
   Motivo = motivo,
    UsuarioId = usuarioId
       };

  _context.Set<MovimentacaoEstoque>().Add(movimentacao);

       await _context.SaveChangesAsync();
          await transaction.CommitAsync();

            return true;
    }
        catch
        {
            await transaction.RollbackAsync();
          throw;
        }
    }

    /// <summary>
    /// Obter movimentações de um medicamento
    /// </summary>
    public async Task<List<MovimentacaoEstoque>> ObterMovimentacoesAsync(string estoqueId)
    {
        return await _context.Set<MovimentacaoEstoque>()
     .Where(m => m.EstoqueFarmaciaId == estoqueId)
      .Include(m => m.Usuario)
.Include(m => m.PedidoMedicamento)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
  }

    /// <summary>
    /// Obter estatísticas do estoque
    /// </summary>
    public async Task<EstoqueEstatisticasDto> ObterEstatisticasAsync()
    {
        var estoque = await _context.Set<EstoqueFarmacia>().ToListAsync();
        var dataLimite = DateTime.UtcNow.AddDays(90);

        return new EstoqueEstatisticasDto
        {
            TotalMedicamentos = estoque.Count,
  MedicamentosDisponiveis = estoque.Count(e => e.Status == "DISPONIVEL"),
            MedicamentosEstoqueBaixo = estoque.Count(e => e.QuantidadeAtual <= e.QuantidadeMinima),
            MedicamentosProximosVencimento = estoque.Count(e => e.DataValidade.HasValue && 
            e.DataValidade.Value <= dataLimite),
MedicamentosVencidos = estoque.Count(e => e.DataValidade.HasValue && 
        e.DataValidade.Value < DateTime.UtcNow),
  QuantidadeTotalEstoque = estoque.Sum(e => e.QuantidadeAtual)
        };
    }

    /// <summary>
    /// Alias para ObterTodoEstoqueAsync (compatibilidade)
    /// </summary>
    public Task<List<EstoqueFarmacia>> GetAllAsync() => ObterTodoEstoqueAsync();

    /// <summary>
    /// Criar novo item de estoque (wrapper)
    /// </summary>
    public async Task CreateAsync(EstoqueFarmacia estoque)
    {
     await AdicionarEstoqueAsync(
      estoque.NomeMedicamento,
      estoque.QuantidadeAtual,
    "system",
       estoque.PrincipioAtivo,
      estoque.Concentracao,
      estoque.FormaFarmaceutica,
       estoque.Lote,
      estoque.DataValidade,
estoque.Localizacao,
       estoque.QuantidadeMinima
 );
    }

    /// <summary>
    /// Atualizar item de estoque (wrapper)
    /// </summary>
 public async Task UpdateAsync(EstoqueFarmacia estoque)
    {
        _context.Set<EstoqueFarmacia>().Update(estoque);
  await _context.SaveChangesAsync();
  }

    /// <summary>
    /// Deletar item de estoque
    /// </summary>
    public async Task DeleteAsync(string id)
    {
   var estoque = await _context.Set<EstoqueFarmacia>().FindAsync(id);
 if (estoque != null)
  {
       _context.Set<EstoqueFarmacia>().Remove(estoque);
 await _context.SaveChangesAsync();
 }
    }
}

public class EstoqueEstatisticasDto
{
  public int TotalMedicamentos { get; set; }
    public int MedicamentosDisponiveis { get; set; }
    public int MedicamentosEstoqueBaixo { get; set; }
public int MedicamentosProximosVencimento { get; set; }
    public int MedicamentosVencidos { get; set; }
    public int QuantidadeTotalEstoque { get; set; }
}
