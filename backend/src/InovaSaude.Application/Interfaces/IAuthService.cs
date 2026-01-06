using InovaSaude.Application.DTOs;

namespace InovaSaude.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
    Task<bool> RegisterAsync(CreateUserDto request);
}
