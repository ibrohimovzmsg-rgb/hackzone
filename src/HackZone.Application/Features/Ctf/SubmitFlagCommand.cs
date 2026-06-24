using FluentValidation;
using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace HackZone.Application.Features.Ctf;

public record SubmitFlagCommand(Guid ChallengeId, string Flag) : IRequest<SubmitFlagResponse>;

public class SubmitFlagValidator : AbstractValidator<SubmitFlagCommand>
{
    public SubmitFlagValidator()
    {
        RuleFor(x => x.Flag).NotEmpty().MaximumLength(500);
    }
}

public class SubmitFlagHandler(IUnitOfWork uow, ICurrentUserService currentUser, ICacheService cache)
    : IRequestHandler<SubmitFlagCommand, SubmitFlagResponse>
{
    public async Task<SubmitFlagResponse> Handle(SubmitFlagCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();
        var uid = currentUser.UserId.Value;

        var challenge = await uow.CtfChallenges.Query()
            .FirstOrDefaultAsync(c => c.Id == cmd.ChallengeId && c.IsPublished, ct)
            ?? throw new NotFoundException("Challenge", cmd.ChallengeId);

        var alreadySolved = await uow.FlagSubmissions.Query()
            .AnyAsync(s => s.UserId == uid && s.ChallengeId == cmd.ChallengeId && s.IsCorrect, ct);

        if (alreadySolved)
            return new SubmitFlagResponse(false, null, "Bu challengeni allaqachon yechdingiz!");

        var isCorrect = BC.Verify(cmd.Flag.Trim(), challenge.FlagHash);

        if (isCorrect)
        {
            challenge.IncrementSolve();
            var submission = FlagSubmission.Correct(uid, cmd.ChallengeId, challenge.Points);
            await uow.FlagSubmissions.AddAsync(submission, ct);

            var user = await uow.Users.Query().FirstOrDefaultAsync(u => u.Id == uid, ct);
            user?.AddReputation(challenge.Points);

            await uow.SaveChangesAsync(ct);

            var username = currentUser.Username ?? "unknown";
            var score = await uow.FlagSubmissions.Query()
                .Where(s => s.UserId == uid && s.IsCorrect)
                .SumAsync(s => s.PointsAwarded, ct);
            await cache.UpdateScoreAsync(username, score);

            return new SubmitFlagResponse(true, challenge.Points, "To'\''g'\''ri! +{challenge.Points} ball!");
        }
        else
        {
            await uow.FlagSubmissions.AddAsync(FlagSubmission.Incorrect(uid, cmd.ChallengeId), ct);
            await uow.SaveChangesAsync(ct);
            return new SubmitFlagResponse(false, null, "Noto'\''g'\''ri flag. Qaytadan urining.");
        }
    }
}

