namespace HackZone.Application.Features.Ctf;

public record ChallengeListItem(Guid Id, string Title, string Category,
    string Difficulty, int Points, int SolveCount, bool IsSolved);

public record ChallengeDetail(Guid Id, string Title, string Description,
    string Category, string Difficulty, int Points, string? HintText,
    int SolveCount, bool IsSolved);

public record SubmitFlagRequest(Guid ChallengeId, string Flag);
public record SubmitFlagResponse(bool Correct, int? PointsAwarded, string Message);
public record LeaderboardEntry(int Rank, string Username, string? AvatarUrl, int Score, int SolvedCount);
