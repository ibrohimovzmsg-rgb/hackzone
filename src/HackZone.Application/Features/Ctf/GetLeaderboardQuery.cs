using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Ctf;

public record GetLeaderboardQuery(int Top = 20) : IRequest<List<LeaderboardEntry>>;

public class GetLeaderboardHandler(IUnitOfWork uow, ICacheService cache)
    : IRequestHandler<GetLeaderboardQuery, List<LeaderboardEntry>>
{
    public async Task<List<LeaderboardEntry>> Handle(GetLeaderboardQuery q, CancellationToken ct)
    {
        var cached = await cache.GetLeaderboardAsync(q.Top);
        if (cached.Count > 0)
        {
            return cached.Select((entry, i) => new LeaderboardEntry(
                i + 1, entry.Username, null, (int)entry.Score, 0)).ToList();
        }

        // Fallback: DB dan hisoblash
        var leaders = await uow.FlagSubmissions.Query()
            .Where(s => s.IsCorrect)
            .GroupBy(s => s.UserId)
            .Select(g => new { UserId = g.Key, Score = g.Sum(s => s.PointsAwarded), Count = g.Count() })
            .OrderByDescending(x => x.Score)
            .Take(q.Top)
            .ToListAsync(ct);

        var userIds = leaders.Select(l => l.UserId).ToList();
        var users = await uow.Users.Query()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        return leaders.Select((l, i) =>
        {
            users.TryGetValue(l.UserId, out var user);
            return new LeaderboardEntry(i + 1, user?.Username ?? "unknown",
                user?.AvatarUrl, l.Score, l.Count);
        }).ToList();
    }
}
