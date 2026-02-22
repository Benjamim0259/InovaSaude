using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace InovaSaude.Blazor.Services;

public class AuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string?> GetCurrentUserIdAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        return null;
    }

    public async Task<string?> GetCurrentUserEmailAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        return null;
    }

    public async Task<string?> GetCurrentUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        return null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User?.Identity?.IsAuthenticated == true;
    }
}
