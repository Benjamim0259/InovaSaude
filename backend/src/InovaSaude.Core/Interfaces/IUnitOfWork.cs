namespace InovaSaude.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUbsRepository Ubs { get; }
    IDespesaRepository Despesas { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
