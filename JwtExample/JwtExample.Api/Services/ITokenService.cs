using JwtExample.Api.Models;

namespace JwtExample.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        void RevokeToken(string token);
        bool IsTokenRevoked(string token);
    }
}
