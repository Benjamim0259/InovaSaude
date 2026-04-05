using InovaSaude.Web.Models;

namespace InovaSaude.Web.Services;

public interface IApiService
{
    // Auth
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UserDto> GetCurrentUserAsync();
    
    // UBS
    Task<List<UbsDto>> GetUbsListAsync();
    Task<UbsDto> GetUbsByIdAsync(Guid id);
    Task<UbsDto> CreateUbsAsync(CreateUbsDto dto);
    Task UpdateUbsAsync(Guid id, UpdateUbsDto dto);
    Task DeleteUbsAsync(Guid id);
    
    // Despesas
    Task<List<DespesaDto>> GetDespesasAsync(Guid? ubsId = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<DespesaDto> GetDespesaByIdAsync(Guid id);
    Task<DespesaDto> CreateDespesaAsync(CreateDespesaDto dto);
    Task UpdateDespesaAsync(Guid id, UpdateDespesaDto dto);
    
    // Dashboard
    Task<DashboardDto> GetDashboardDataAsync();
}
