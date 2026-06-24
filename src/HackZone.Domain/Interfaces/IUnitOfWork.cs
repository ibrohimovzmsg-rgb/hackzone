using HackZone.Domain.Entities;

namespace HackZone.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<Course> Courses { get; }
    IRepository<Lesson> Lessons { get; }
    IRepository<Enrollment> Enrollments { get; }
    IRepository<LessonProgress> LessonProgresses { get; }
    IRepository<Lab> Labs { get; }
    IRepository<LabInstance> LabInstances { get; }
    IRepository<CtfChallenge> CtfChallenges { get; }
    IRepository<FlagSubmission> FlagSubmissions { get; }
    IRepository<Certificate> Certificates { get; }
    IRepository<ForumCategory> ForumCategories { get; }
    IRepository<ForumPost> ForumPosts { get; }
    IRepository<ForumReply> ForumReplies { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
