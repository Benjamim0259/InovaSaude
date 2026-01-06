using InovaSaude.Core.Entities;

namespace InovaSaude.Core.Interfaces;

public interface IUbsRepository : IRepository<UBS>
{
    Task<IEnumerable<UBS>> GetByMunicipioIdAsync(Guid municipioId);
}
