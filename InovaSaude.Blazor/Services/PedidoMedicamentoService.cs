using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class PedidoMedicamentoService
{
    private readonly ApplicationDbContext _context;

    public PedidoMedicamentoService(ApplicationDbContext context)
  {
 _context = context;
    }

    /// <summary>
    /// Criar novo pedido de medicamentos
    /// </summary>
    public async Task<PedidoMedicamento> CriarPedidoAsync(
        string ubsId,
        string usuarioId,
        List<ItemPedidoMedicamento> itens,
  string? observacoes = null,
      DateTime? dataNecessidade = null,
      string prioridade = "NORMAL")
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

 try
      {
          // Gerar número do pedido
 var numeroPedido = await GerarNumeroPedidoAsync();

var pedido = new PedidoMedicamento
     {
     NumeroPedido = numeroPedido,
          UbsSolicitanteId = ubsId,
       UsuarioCriacaoId = usuarioId,
                DataPedido = DateTime.UtcNow,
        DataNecessidade = dataNecessidade,
       Status = "PENDENTE",
    Prioridade = prioridade,
  Observacoes = observacoes
       };

            _context.Set<PedidoMedicamento>().Add(pedido);
            await _context.SaveChangesAsync();

    // Adicionar itens
            foreach (var item in itens)
            {
                item.PedidoMedicamentoId = pedido.Id;
     _context.Set<ItemPedidoMedicamento>().Add(item);
            }

await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return pedido;
        }
      catch
        {
     await transaction.RollbackAsync();
            throw;
        }
    }

 /// <summary>
    /// Gerar número sequencial do pedido
    /// </summary>
    private async Task<string> GerarNumeroPedidoAsync()
    {
  var ano = DateTime.UtcNow.Year;
        var ultimoPedido = await _context.Set<PedidoMedicamento>()
            .Where(p => p.NumeroPedido.StartsWith($"PED-{ano}-"))
            .OrderByDescending(p => p.CreatedAt)
  .FirstOrDefaultAsync();

  int proximoNumero = 1;
        if (ultimoPedido != null)
     {
            var partes = ultimoPedido.NumeroPedido.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
  {
                proximoNumero = numero + 1;
            }
    }

        return $"PED-{ano}-{proximoNumero:D6}"; // Ex: PED-2025-000001
  }

    /// <summary>
    /// Obter todos os pedidos
    /// </summary>
    public async Task<List<PedidoMedicamento>> ObterTodosPedidosAsync(string? ubsId = null, string? status = null)
    {
        var query = _context.Set<PedidoMedicamento>()
        .Include(p => p.UbsSolicitante)
          .Include(p => p.UsuarioCriacao)
  .Include(p => p.UsuarioAprovacao)
         .Include(p => p.Itens)
    .AsQueryable();

     if (!string.IsNullOrEmpty(ubsId))
        {
            query = query.Where(p => p.UbsSolicitanteId == ubsId);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(p => p.Status == status);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    /// <summary>
  /// Obter pedido por ID
    /// </summary>
    public async Task<PedidoMedicamento?> ObterPedidoPorIdAsync(string id)
    {
        return await _context.Set<PedidoMedicamento>()
         .Include(p => p.UbsSolicitante)
  .Include(p => p.UsuarioCriacao)
      .Include(p => p.UsuarioAprovacao)
            .Include(p => p.UsuarioEntrega)
            .Include(p => p.Itens)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Aprovar pedido
    /// </summary>
    public async Task<bool> AprovarPedidoAsync(
        string pedidoId,
        string usuarioId,
        Dictionary<string, int> quantidadesAprovadas)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

     try
        {
 var pedido = await ObterPedidoPorIdAsync(pedidoId);
            if (pedido == null || pedido.Status != "PENDENTE")
       return false;

            // Verificar estoque disponível
        foreach (var item in pedido.Itens)
         {
             if (quantidadesAprovadas.TryGetValue(item.Id, out int qtdAprovada))
      {
    var estoque = await _context.Set<EstoqueFarmacia>()
  .FirstOrDefaultAsync(e => e.NomeMedicamento == item.NomeMedicamento &&
      e.Status == "DISPONIVEL");

       if (estoque == null || estoque.QuantidadeAtual < qtdAprovada)
        {
 item.QuantidadeAprovada = estoque?.QuantidadeAtual ?? 0;
    item.Justificativa = "Estoque insuficiente";
         }
           else
          {
   item.QuantidadeAprovada = qtdAprovada;
              }
    }
       }

            pedido.Status = "APROVADO";
       pedido.UsuarioAprovacaoId = usuarioId;
      pedido.DataAprovacao = DateTime.UtcNow;
            pedido.UpdatedAt = DateTime.UtcNow;

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
    /// Rejeitar pedido
    /// </summary>
    public async Task<bool> RejeitarPedidoAsync(string pedidoId, string usuarioId, string motivo)
    {
        var pedido = await _context.Set<PedidoMedicamento>().FindAsync(pedidoId);
   if (pedido == null || pedido.Status != "PENDENTE")
 return false;

        pedido.Status = "REJEITADO";
        pedido.UsuarioAprovacaoId = usuarioId;
        pedido.MotivoRejeicao = motivo;
        pedido.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Marcar pedido como entregue
    /// </summary>
    public async Task<bool> EntregarPedidoAsync(string pedidoId, string usuarioId)
  {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
     {
            var pedido = await ObterPedidoPorIdAsync(pedidoId);
        if (pedido == null || pedido.Status != "APROVADO")
    return false;

            // Dar baixa no estoque
  foreach (var item in pedido.Itens)
         {
            if (item.QuantidadeAprovada > 0)
       {
      var estoque = await _context.Set<EstoqueFarmacia>()
 .FirstOrDefaultAsync(e => e.NomeMedicamento == item.NomeMedicamento);

          if (estoque != null)
    {
  // Criar movimentação de saída
            var movimentacao = new MovimentacaoEstoque
         {
         EstoqueFarmaciaId = estoque.Id,
      Tipo = "SAIDA",
             Quantidade = -item.QuantidadeAprovada.Value,
 QuantidadeAnterior = estoque.QuantidadeAtual,
QuantidadeApos = estoque.QuantidadeAtual - item.QuantidadeAprovada.Value,
       Motivo = $"Entrega pedido {pedido.NumeroPedido} para {pedido.UbsSolicitante.Nome}",
          NumeroDocumento = pedido.NumeroPedido,
    PedidoMedicamentoId = pedido.Id,
    UsuarioId = usuarioId
         };

    _context.Set<MovimentacaoEstoque>().Add(movimentacao);

               // Atualizar quantidade em estoque
      estoque.QuantidadeAtual -= item.QuantidadeAprovada.Value;
     estoque.UltimaMovimentacao = DateTime.UtcNow;
estoque.UpdatedAt = DateTime.UtcNow;

          // Atualizar status se estoque baixo
           if (estoque.QuantidadeAtual <= estoque.QuantidadeMinima)
         {
       estoque.Status = "BAIXO_ESTOQUE";
          }

       item.QuantidadeEntregue = item.QuantidadeAprovada;
     }
   }
       }

        pedido.Status = "ENTREGUE";
            pedido.UsuarioEntregaId = usuarioId;
     pedido.DataEntrega = DateTime.UtcNow;
        pedido.UpdatedAt = DateTime.UtcNow;

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
    /// Obter estatísticas de pedidos
    /// </summary>
    public async Task<PedidoEstatisticasDto> ObterEstatisticasAsync(DateTime inicio, DateTime fim, string? ubsId = null)
    {
        var query = _context.Set<PedidoMedicamento>()
 .Where(p => p.CreatedAt >= inicio && p.CreatedAt <= fim);

        if (!string.IsNullOrEmpty(ubsId))
        {
  query = query.Where(p => p.UbsSolicitanteId == ubsId);
        }

    var pedidos = await query.Include(p => p.Itens).ToListAsync();

        return new PedidoEstatisticasDto
     {
            TotalPedidos = pedidos.Count,
    PedidosPendentes = pedidos.Count(p => p.Status == "PENDENTE"),
    PedidosAprovados = pedidos.Count(p => p.Status == "APROVADO"),
            PedidosEntregues = pedidos.Count(p => p.Status == "ENTREGUE"),
     PedidosRejeitados = pedidos.Count(p => p.Status == "REJEITADA"),
            TotalItens = pedidos.Sum(p => p.Itens.Sum(i => i.QuantidadeSolicitada)),
            TotalItensEntregues = pedidos.Sum(p => p.Itens.Sum(i => i.QuantidadeEntregue ?? 0)),
            TempoMedioAprovacao = pedidos
            .Where(p => p.DataAprovacao.HasValue)
        .Average(p => (p.DataAprovacao!.Value - p.DataPedido).TotalHours)
    };
    }
}

public class PedidoEstatisticasDto
{
    public int TotalPedidos { get; set; }
    public int PedidosPendentes { get; set; }
    public int PedidosAprovados { get; set; }
    public int PedidosEntregues { get; set; }
    public int PedidosRejeitados { get; set; }
    public int TotalItens { get; set; }
    public int TotalItensEntregues { get; set; }
    public double TempoMedioAprovacao { get; set; }
}
