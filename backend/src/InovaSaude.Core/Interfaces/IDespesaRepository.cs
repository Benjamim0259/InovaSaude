using InovaSaude.Core.Entities;

namespace InovaSaude.Core.Interfaces;

public interface IDespesaRepository : IRepository<Despesa>
{
    Task<IEnumerable<Despesa>> GetByUbsIdAsync(Guid ubsId);
    Task<IEnumerable<Despesa>> GetByPeriodAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Despesa>> GetByUbsAndPeriodAsync(Guid ubsId, DateTime startDate, DateTime endDate);
}
