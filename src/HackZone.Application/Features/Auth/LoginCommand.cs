using FluentValidation;
using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace HackZone.Application.Features.Auth;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginHandler(IUnitOfWork uow, IJwtService jwt) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var email = cmd.Email.Trim().ToLower();
        var user = await uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct)
            ?? throw new UnauthorizedException("Email yoki parol noto'g'ri.");

        if (user.IsLockedOut())
            throw new UnauthorizedException($"Hisob vaqtincha bloklangan. {user.LockedUntil:HH:mm} gacha kuting.");

        if (!BC.Verify(cmd.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await uow.SaveChangesAsync(ct);
            throw new UnauthorizedException("Email yoki parol noto'g'ri.");
        }

        user.RecordSuccessfulLogin();
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();

        var accessToken = jwt.GenerateAccessToken(user, roles);
        var (refreshToken, expiresAt) = jwt.GenerateRefreshToken();

        var rt = RefreshToken.Create(user.Id, refreshToken, expiresAt);
        await uow.RefreshTokens.AddAsync(rt, ct);
        await uow.SaveChangesAsync(ct);

        return new AuthResponse(
            new TokenResponse(accessToken, refreshToken, expiresAt),
            new UserProfileResponse(
                user.Id, user.Username, user.Email, user.DisplayName,
                user.AvatarUrl, user.Bio, user.Country,
                user.ReputationPoints, user.EmailConfirmed, roles, user.CreatedAt));
    }
}
