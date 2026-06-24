namespace HackZone.Application.Features.Auth;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record UpdateProfileRequest(string? DisplayName, string? Bio, string? Country);

public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
public record UserProfileResponse(
    Guid Id, string Username, string Email,
    string? DisplayName, string? AvatarUrl, string? Bio, string? Country,
    int ReputationPoints, bool EmailConfirmed, string[] Roles, DateTime CreatedAt);
public record AuthResponse(TokenResponse Token, UserProfileResponse User);
