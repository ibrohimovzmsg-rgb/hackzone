namespace HackZone.Web.Models;

public record UserProfile(Guid Id, string Username, string Email, string? FullName, string? Bio,
    int ReputationPoints, List<string> Roles);

public record DashboardStats(int TotalUsers, int TotalCourses, int TotalChallenges, int ActiveLabs, int TotalCertificates);
public record AdminUser(Guid Id, string Username, string Email, string Status, int ReputationPoints);

public record CertItem(Guid Id, string CourseTitle, string Code, DateTime IssuedAt);

public record CourseItem(Guid Id, string Title, string Slug, string Description, string Difficulty,
    int LessonCount, int EnrollmentCount);

public record ChallengeItem(Guid Id, string Title, string Description, string Category, string Difficulty,
    int Points, bool IsSolved, int SolveCount);
public record FlagResult(bool Correct, int Points);

public record ForumCat(Guid Id, string Name, string Slug, int PostCount);
public record ForumPostItem(Guid Id, string Title, string AuthorUsername, DateTime CreatedAt, int ReplyCount, int ViewCount);

public record LabItem(Guid Id, string Title, string Description, string Difficulty, int ExpiryMinutes);
public record LabStarted(string ContainerId, string AccessUrl);

public record LeaderItem(int Rank, string Username, long Score, int SolvedCount);
