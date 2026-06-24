using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;

namespace HackZone.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public IRepository<User> Users { get; } = new Repository<User>(db);
    public IRepository<Role> Roles { get; } = new Repository<Role>(db);
public IRepository<RefreshToken> RefreshTokens { get; } = new Repository<RefreshToken>(db);
    public IRepository<Course> Courses { get; } = new Repository<Course>(db);
    public IRepository<Lesson> Lessons { get; } = new Repository<Lesson>(db);
    public IRepository<Enrollment> Enrollments { get; } = new Repository<Enrollment>(db);
    public IRepository<LessonProgress> LessonProgresses { get; } = new Repository<LessonProgress>(db);
    public IRepository<Lab> Labs { get; } = new Repository<Lab>(db);
    public IRepository<LabInstance> LabInstances { get; } = new Repository<LabInstance>(db);
    public IRepository<CtfChallenge> CtfChallenges { get; } = new Repository<CtfChallenge>(db);
    public IRepository<FlagSubmission> FlagSubmissions { get; } = new Repository<FlagSubmission>(db);
    public IRepository<Certificate> Certificates { get; } = new Repository<Certificate>(db);
    public IRepository<ForumCategory> ForumCategories { get; } = new Repository<ForumCategory>(db);
    public IRepository<ForumPost> ForumPosts { get; } = new Repository<ForumPost>(db);
    public IRepository<ForumReply> ForumReplies { get; } = new Repository<ForumReply>(db);
    public IRepository<Notification> Notifications { get; } = new Repository<Notification>(db);
    public IRepository<AuditLog> AuditLogs { get; } = new Repository<AuditLog>(db);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public void Dispose() => db.Dispose();
}
