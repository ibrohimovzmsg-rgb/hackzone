using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Ctf;

public record GetChallengesQuery : IRequest<List<ChallengeListItem>>;

public class GetChallengesHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetChallengesQuery, List<ChallengeListItem>>
{
    public async Task<List<ChallengeListItem>> Handle(GetChallengesQuery _, CancellationToken ct)
    {
        var uid = currentUser.UserId;
        var solved = uid.HasValue
            ? await uow.FlagSubmissions.Query()
                .Where(s => s.UserId == uid.Value && s.IsCorrect)
                .Select(s => s.ChallengeId).ToListAsync(ct)
            : [];

        var challenges = await uow.CtfChallenges.Query()
            .Where(c => c.IsPublished)
            .OrderBy(c => c.Difficulty).ThenByDescending(c => c.Points)
            .ToListAsync(ct);

        return challenges.Select(c => new ChallengeListItem(
            c.Id, c.Title, c.Category.ToString(), c.Difficulty.ToString(),
            c.Points, c.SolveCount, solved.Contains(c.Id))).ToList();
    }
}
