using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;

public class RefreshTokenHandler(IUnitOfWork uow, IJwtService jwt) : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var rt = await uow.RefreshTokens.Query()
            .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken, ct)
            ?? throw new UnauthorizedException("Refresh token topilmadi.");

        if (!rt.IsActive) throw new UnauthorizedException("Refresh token amal qilmaydi.");

        var user = await uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == rt.UserId, ct)
            ?? throw new UnauthorizedException("Foydalanuvchi topilmadi.");

        rt.Revoke();

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        var accessToken = jwt.GenerateAccessToken(user, roles);
        var (newRefresh, expiresAt) = jwt.GenerateRefreshToken();

        var newRt = RefreshToken.Create(user.Id, newRefresh, expiresAt);
        await uow.RefreshTokens.AddAsync(newRt, ct);
        await uow.SaveChangesAsync(ct);

        return new TokenResponse(accessToken, newRefresh, expiresAt);
    }
}
