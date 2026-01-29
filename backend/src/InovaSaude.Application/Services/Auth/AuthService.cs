using InovaSaude.Application.DTOs;
using InovaSaude.Application.Interfaces;
using InovaSaude.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace InovaSaude.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Store refresh token (in production, store in database with expiration)
        // For now, we'll skip this and just return it

        return new LoginResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = await MapToUserDto(user, roles)
        };
    }

    public Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // In production, validate refresh token against database
        // For now, this is a simplified implementation
        return Task.FromException<LoginResponseDto>(
            new NotImplementedException("Refresh token not implemented yet"));
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        return await MapToUserDto(user, roles);
    }

    public async Task<bool> RegisterAsync(CreateUserDto request)
    {
        var user = new Usuario
        {
            UserName = request.Email,
            Email = request.Email,
            Nome = request.Nome,
            Cpf = request.Cpf,
            MunicipioId = request.MunicipioId,
            UbsId = request.UbsId,
            EmailConfirmed = true // For MVP, auto-confirm emails
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);

        return true;
    }

    private Task<UserDto> MapToUserDto(Usuario user, IList<string> roles)
    {
        return Task.FromResult(new UserDto
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email ?? string.Empty,
            Cpf = user.Cpf,
            Roles = roles,
            MunicipioId = user.MunicipioId,
            UbsId = user.UbsId
        });
    }
}
