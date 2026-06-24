using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Auth;

public record GetCurrentUserQuery : IRequest<UserProfileResponse>;

public class GetCurrentUserHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetCurrentUserQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetCurrentUserQuery _, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var user = await uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
            ?? throw new NotFoundException("User", currentUser.UserId);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        return new UserProfileResponse(
            user.Id, user.Username, user.Email, user.DisplayName,
            user.AvatarUrl, user.Bio, user.Country,
            user.ReputationPoints, user.EmailConfirmed, roles, user.CreatedAt);
    }
}
