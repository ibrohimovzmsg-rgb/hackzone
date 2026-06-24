using HackZone.Domain.Common;
using HackZone.Domain.Enums;

namespace HackZone.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string? DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public string? Country { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
    public bool EmailConfirmed { get; private set; }
    public string? EmailConfirmationToken { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int ReputationPoints { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    public static User Create(string username, string email, string passwordHash) => new()
    {
        Username = username.Trim().ToLower(),
        Email = email.Trim().ToLower(),
        PasswordHash = passwordHash,
        DisplayName = username,
        EmailConfirmationToken = Guid.NewGuid().ToString("N")
    };

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmationToken = null;
        Status = UserStatus.Active;
        SetUpdated();
    }

    public void UpdateProfile(string? displayName, string? bio, string? country, string? avatarUrl)
    {
        if (displayName is not null) DisplayName = displayName;
        if (bio is not null) Bio = bio;
        if (country is not null) Country = country;
        if (avatarUrl is not null) AvatarUrl = avatarUrl;
        SetUpdated();
    }

    public void SetPasswordHash(string hash) { PasswordHash = hash; SetUpdated(); }

    public bool IsLockedOut() => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        SetUpdated();
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastLoginAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void AddReputation(int points) { ReputationPoints += points; SetUpdated(); }
    public void Ban() { Status = UserStatus.Banned; SetUpdated(); }
    public void Activate() { Status = UserStatus.Active; SetUpdated(); }

    public void AddRole(Guid roleId) => _userRoles.Add(UserRole.Create(Id, roleId));
    public void AddRefreshToken(RefreshToken token) => _refreshTokens.Add(token);

    public string GenerateEmailToken()
    {
        EmailConfirmationToken = Guid.NewGuid().ToString("N");
        SetUpdated();
        return EmailConfirmationToken;
    }
}
