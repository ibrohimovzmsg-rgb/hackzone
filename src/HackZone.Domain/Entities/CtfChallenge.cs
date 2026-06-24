using HackZone.Domain.Common;
using HackZone.Domain.Enums;

namespace HackZone.Domain.Entities;

public class CtfChallenge : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public ChallengeCategory Category { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public int Points { get; private set; }
    public string FlagHash { get; private set; } = default!;
    public string? HintText { get; private set; }
    public bool IsPublished { get; private set; }
    public int SolveCount { get; private set; }

    private readonly List<FlagSubmission> _submissions = [];
    public IReadOnlyCollection<FlagSubmission> Submissions => _submissions.AsReadOnly();

    private CtfChallenge() { }

    public static CtfChallenge Create(string title, string description,
        ChallengeCategory category, Difficulty difficulty, int points, string flagHash) => new()
    {
        Title = title, Description = description, Category = category,
        Difficulty = difficulty, Points = points, FlagHash = flagHash
    };

    public void Publish() { IsPublished = true; SetUpdated(); }
    public void SetHint(string hint) { HintText = hint; SetUpdated(); }
    public void IncrementSolve() { SolveCount++; SetUpdated(); }
}

public class FlagSubmission : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ChallengeId { get; private set; }
    public bool IsCorrect { get; private set; }
    public int PointsAwarded { get; private set; }
    public CtfChallenge Challenge { get; private set; } = default!;

    private FlagSubmission() { }

    public static FlagSubmission Correct(Guid userId, Guid challengeId, int points) =>
        new() { UserId = userId, ChallengeId = challengeId, IsCorrect = true, PointsAwarded = points };

    public static FlagSubmission Incorrect(Guid userId, Guid challengeId) =>
        new() { UserId = userId, ChallengeId = challengeId };
}
