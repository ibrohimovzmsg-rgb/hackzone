using HackZone.Domain.Common;
using HackZone.Domain.Enums;

namespace HackZone.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ThumbnailUrl { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public string Category { get; private set; } = default!;
    public bool IsPublished { get; private set; }
    public int OrderIndex { get; private set; }

    private readonly List<Lesson> _lessons = [];
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    private readonly List<Enrollment> _enrollments = [];
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Course() { }

    public static Course Create(string title, string description, string category, Difficulty difficulty) => new()
    {
        Title = title,
        Slug = title.ToLower().Replace(" ", "-"),
        Description = description,
        Category = category,
        Difficulty = difficulty
    };

    public void Publish() { IsPublished = true; SetUpdated(); }
    public void Unpublish() { IsPublished = false; SetUpdated(); }
    public void SetThumbnail(string url) { ThumbnailUrl = url; SetUpdated(); }
}

public class Lesson : BaseEntity
{
    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public string? VideoUrl { get; private set; }
    public int OrderIndex { get; private set; }
    public int DurationMinutes { get; private set; }
    public bool IsPublished { get; private set; }
    public Course Course { get; private set; } = default!;

    private Lesson() { }

    public static Lesson Create(Guid courseId, string title, string content, int order, int durationMins) => new()
    {
        CourseId = courseId, Title = title, Content = content,
        OrderIndex = order, DurationMinutes = durationMins
    };

    public void Publish() { IsPublished = true; SetUpdated(); }
}

public class Enrollment : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime EnrolledAt { get; private set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; private set; }
    public int ProgressPercent { get; private set; }
    public Course Course { get; private set; } = default!;

    private Enrollment() { }

    public static Enrollment Create(Guid userId, Guid courseId) =>
        new() { UserId = userId, CourseId = courseId };

    public void UpdateProgress(int percent)
    {
        ProgressPercent = Math.Clamp(percent, 0, 100);
        if (ProgressPercent == 100) CompletedAt ??= DateTime.UtcNow;
        SetUpdated();
    }
}

public class LessonProgress : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public DateTime CompletedAt { get; private set; } = DateTime.UtcNow;
    public Lesson Lesson { get; private set; } = default!;

    private LessonProgress() { }

    public static LessonProgress Create(Guid userId, Guid lessonId) =>
        new() { UserId = userId, LessonId = lessonId };
}
