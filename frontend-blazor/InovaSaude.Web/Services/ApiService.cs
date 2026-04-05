using System.Net.Http.Json;
using InovaSaude.Web.Models;

namespace InovaSaude.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient http, ILogger<ApiService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResponse>() 
            ?? throw new Exception("Invalid response");
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        return await _http.GetFromJsonAsync<UserDto>("/api/auth/me") 
            ?? throw new Exception("User not found");
    }

    public async Task<List<UbsDto>> GetUbsListAsync()
    {
        return await _http.GetFromJsonAsync<List<UbsDto>>("/api/ubs") ?? new();
    }

    public async Task<UbsDto> GetUbsByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<UbsDto>($"/api/ubs/{id}") 
            ?? throw new Exception("UBS not found");
    }

    public async Task<UbsDto> CreateUbsAsync(CreateUbsDto dto)
    {
        var response = await _http.PostAsJsonAsync("/api/ubs", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UbsDto>() 
            ?? throw new Exception("Invalid response");
    }

    public async Task UpdateUbsAsync(Guid id, UpdateUbsDto dto)
    {
        var response = await _http.PutAsJsonAsync($"/api/ubs/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUbsAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"/api/ubs/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<DespesaDto>> GetDespesasAsync(Guid? ubsId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var query = new List<string>();
        if (ubsId.HasValue) query.Add($"ubsId={ubsId}");
        if (dataInicio.HasValue) query.Add($"dataInicio={dataInicio:yyyy-MM-dd}");
        if (dataFim.HasValue) query.Add($"dataFim={dataFim:yyyy-MM-dd}");
        
        var url = "/api/despesas" + (query.Any() ? "?" + string.Join("&", query) : "");
        return await _http.GetFromJsonAsync<List<DespesaDto>>(url) ?? new();
    }

    public async Task<DespesaDto> GetDespesaByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<DespesaDto>($"/api/despesas/{id}") 
            ?? throw new Exception("Despesa not found");
    }

    public async Task<DespesaDto> CreateDespesaAsync(CreateDespesaDto dto)
    {
        var response = await _http.PostAsJsonAsync("/api/despesas", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DespesaDto>() 
            ?? throw new Exception("Invalid response");
    }

    public async Task UpdateDespesaAsync(Guid id, UpdateDespesaDto dto)
    {
        var response = await _http.PutAsJsonAsync($"/api/despesas/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        return await _http.GetFromJsonAsync<DashboardDto>("/api/relatorios/dashboard") 
            ?? throw new Exception("Dashboard data not found");
    }
}
