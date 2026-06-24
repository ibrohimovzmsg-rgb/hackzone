using FluentValidation;
using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace HackZone.Application.Features.Auth;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<AuthResponse>;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(3).MaximumLength(30)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username faqat harf, raqam, _ yoki - bo'\''lishi mumkin.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Kamida 1 ta katta harf kerak.")
            .Matches("[0-9]").WithMessage("Kamida 1 ta raqam kerak.");
    }
}

public class RegisterHandler(IUnitOfWork uow, IJwtService jwt, IEmailService email)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var emailLower = cmd.Email.Trim().ToLower();
        var usernameLower = cmd.Username.Trim().ToLower();

        if (await uow.Users.Query().AnyAsync(u => u.Email == emailLower, ct))
            throw new ConflictException("Bu email allaqachon ro'\''yxatdan o'\''tgan.");

        if (await uow.Users.Query().AnyAsync(u => u.Username == usernameLower, ct))
            throw new ConflictException("Bu username band.");

        var hash = BC.HashPassword(cmd.Password);
        var user = User.Create(cmd.Username, cmd.Email, hash);

        var studentRole = await uow.Roles.Query()
            .FirstOrDefaultAsync(r => r.Name == Role.Names.Student, ct)
            ?? throw new BusinessException("Student roli topilmadi.");

        user.AddRole(studentRole.Id);
        await uow.Users.AddAsync(user, ct);

        var token = user.GenerateEmailToken();
        await uow.SaveChangesAsync(ct);

        try { await email.SendConfirmationEmailAsync(user.Email, user.Username, token, ct); }
        catch { /* email yuborish ixtiyoriy */ }

        return BuildAuthResponse(user, jwt, ["Student"]);
    }

    internal static AuthResponse BuildAuthResponse(User user, IJwtService jwt, string[] roles)
    {
        var accessToken = jwt.GenerateAccessToken(user, roles);
        var (refreshToken, expiresAt) = jwt.GenerateRefreshToken();

        var rt = RefreshToken.Create(user.Id, refreshToken, expiresAt);
        user.AddRefreshToken(rt);

        return new AuthResponse(
            new TokenResponse(accessToken, refreshToken, expiresAt),
            new UserProfileResponse(
                user.Id, user.Username, user.Email, user.DisplayName,
                user.AvatarUrl, user.Bio, user.Country,
                user.ReputationPoints, user.EmailConfirmed, roles, user.CreatedAt));
    }
}
