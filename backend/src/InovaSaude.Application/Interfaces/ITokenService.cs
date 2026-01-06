using InovaSaude.Core.Entities;

namespace InovaSaude.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Usuario user, IList<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
