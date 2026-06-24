using HackZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();

    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<LabInstance> LabInstances => Set<LabInstance>();

    public DbSet<CtfChallenge> CtfChallenges => Set<CtfChallenge>();
    public DbSet<FlagSubmission> FlagSubmissions => Set<FlagSubmission>();

    public DbSet<Certificate> Certificates => Set<Certificate>();

    public DbSet<ForumCategory> ForumCategories => Set<ForumCategory>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
    public DbSet<ForumReply> ForumReplies => Set<ForumReply>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdated();
        }
        return base.SaveChangesAsync(ct);
    }
}
