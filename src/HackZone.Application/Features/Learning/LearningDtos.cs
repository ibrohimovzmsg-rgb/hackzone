namespace HackZone.Application.Features.Learning;

public record CourseListItem(Guid Id, string Title, string Description, string Slug,
    string Difficulty, string Category, string? ThumbnailUrl,
    int LessonCount, bool IsEnrolled, int ProgressPercent, int EnrollmentCount);

public record CourseDetail(Guid Id, string Title, string Description, string Difficulty,
    string Category, string? ThumbnailUrl, List<LessonItem> Lessons,
    bool IsEnrolled, int ProgressPercent);

public record LessonItem(Guid Id, string Title, int OrderIndex, int DurationMinutes, bool IsCompleted);

public record LessonDetail(Guid Id, string Title, string Content, string? VideoUrl,
    int OrderIndex, int DurationMinutes, bool IsCompleted);
