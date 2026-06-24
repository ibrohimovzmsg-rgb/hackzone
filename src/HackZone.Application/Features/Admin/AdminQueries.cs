using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Admin;

public record DashboardStats(int TotalUsers, int TotalCourses, int TotalChallenges, int TotalLabs, int TotalCertificates);

public record GetDashboardStatsQuery : IRequest<DashboardStats>;

public class GetDashboardStatsHandler(IUnitOfWork uow)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStats>
{
    public async Task<DashboardStats> Handle(GetDashboardStatsQuery _, CancellationToken ct)
    {
        return new DashboardStats(
            await uow.Users.Query().CountAsync(ct),
            await uow.Courses.Query().CountAsync(ct),
            await uow.CtfChallenges.Query().CountAsync(ct),
            await uow.Labs.Query().CountAsync(ct),
            await uow.Certificates.Query().CountAsync(ct));
    }
}

public record AdminUserItem(Guid Id, string Username, string Email, string Status,
    int ReputationPoints, string[] Roles, DateTime CreatedAt);

public record GetUsersQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<List<AdminUserItem>>;

public class GetUsersHandler(IUnitOfWork uow)
    : IRequestHandler<GetUsersQuery, List<AdminUserItem>>
{
    public async Task<List<AdminUserItem>> Handle(GetUsersQuery q, CancellationToken ct)
    {
        var query = uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(u => u.Username.Contains(q.Search) || u.Email.Contains(q.Search));

        var users = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize)
            .ToListAsync(ct);

        return users.Select(u => new AdminUserItem(
            u.Id, u.Username, u.Email, u.Status.ToString(),
            u.ReputationPoints,
            u.UserRoles.Select(ur => ur.Role.Name).ToArray(),
            u.CreatedAt)).ToList();
    }
}

public record BanUserCommand(Guid UserId) : IRequest;

public class BanUserHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<BanUserCommand>
{
    public async Task Handle(BanUserCommand cmd, CancellationToken ct)
    {
        if (!currentUser.IsInRole("Admin")) throw new ForbiddenException();

        var user = await uow.Users.Query().FirstOrDefaultAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException("User", cmd.UserId);

        user.Ban();
        await uow.SaveChangesAsync(ct);
    }
}
