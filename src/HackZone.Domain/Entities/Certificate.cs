using HackZone.Domain.Common;

namespace HackZone.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Code { get; private set; } = default!;
    public string CourseTitle { get; private set; } = default!;
    public DateTime IssuedAt { get; private set; } = DateTime.UtcNow;
    public Course Course { get; private set; } = default!;
    public User User { get; private set; } = default!;

    private Certificate() { }

    public static Certificate Issue(Guid userId, Guid courseId, string courseTitle) => new()
    {
        UserId = userId,
        CourseId = courseId,
        CourseTitle = courseTitle,
        Code = $"HZ-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
    };
}
