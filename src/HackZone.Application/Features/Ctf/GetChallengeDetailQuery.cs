using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Ctf;

public record GetChallengeDetailQuery(Guid Id) : IRequest<ChallengeDetail>;

public class GetChallengeDetailHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetChallengeDetailQuery, ChallengeDetail>
{
    public async Task<ChallengeDetail> Handle(GetChallengeDetailQuery q, CancellationToken ct)
    {
        var challenge = await uow.CtfChallenges.Query()
            .FirstOrDefaultAsync(c => c.Id == q.Id && c.IsPublished, ct)
            ?? throw new NotFoundException("Challenge", q.Id);

        var uid = currentUser.UserId;
        var isSolved = uid.HasValue && await uow.FlagSubmissions.Query()
            .AnyAsync(s => s.UserId == uid.Value && s.ChallengeId == q.Id && s.IsCorrect, ct);

        return new ChallengeDetail(
            challenge.Id, challenge.Title, challenge.Description,
            challenge.Category.ToString(), challenge.Difficulty.ToString(),
            challenge.Points, challenge.HintText, challenge.SolveCount, isSolved);
    }
}
