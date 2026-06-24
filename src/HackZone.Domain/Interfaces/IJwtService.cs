using HackZone.Domain.Entities;

namespace HackZone.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
}
