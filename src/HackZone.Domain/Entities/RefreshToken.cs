using HackZone.Domain.Common;

namespace HackZone.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? IpAddress { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string? ip = null) =>
        new() { UserId = userId, Token = token, ExpiresAt = expiresAt, IpAddress = ip };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke() { IsRevoked = true; SetUpdated(); }
}
