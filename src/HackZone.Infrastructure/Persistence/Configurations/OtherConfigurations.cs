using HackZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackZone.Infrastructure.Persistence.Configurations;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> b)
    {
        b.HasKey(l => l.Id);
        b.Property(l => l.Title).HasMaxLength(200).IsRequired();
        b.Property(l => l.DockerImage).HasMaxLength(500).IsRequired();
    }
}

public class LabInstanceConfiguration : IEntityTypeConfiguration<LabInstance>
{
    public void Configure(EntityTypeBuilder<LabInstance> b)
    {
        b.HasKey(i => i.Id);
        b.HasOne(i => i.Lab).WithMany()
            .HasForeignKey(i => i.LabId).OnDelete(DeleteBehavior.Restrict);
        b.Property(i => i.ContainerId).HasMaxLength(100).IsRequired();
    }
}

public class CtfChallengeConfiguration : IEntityTypeConfiguration<CtfChallenge>
{
    public void Configure(EntityTypeBuilder<CtfChallenge> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Title).HasMaxLength(200).IsRequired();
        b.Property(c => c.FlagHash).HasMaxLength(100).IsRequired();
        b.HasMany(c => c.Submissions).WithOne(s => s.Challenge)
            .HasForeignKey(s => s.ChallengeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FlagSubmissionConfiguration : IEntityTypeConfiguration<FlagSubmission>
{
    public void Configure(EntityTypeBuilder<FlagSubmission> b)
    {
        b.HasKey(s => s.Id);
        b.HasIndex(s => new { s.UserId, s.ChallengeId, s.IsCorrect });
    }
}

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> b)
    {
        b.HasKey(c => c.Id);
        b.HasIndex(c => c.Code).IsUnique();
        b.Property(c => c.Code).HasMaxLength(50).IsRequired();
        b.Property(c => c.CourseTitle).HasMaxLength(200).IsRequired();
        b.HasOne(c => c.Course).WithMany()
            .HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(c => c.User).WithMany()
            .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ForumCategoryConfiguration : IEntityTypeConfiguration<ForumCategory>
{
    public void Configure(EntityTypeBuilder<ForumCategory> b)
    {
        b.HasKey(c => c.Id);
        b.HasIndex(c => c.Slug).IsUnique();
        b.Property(c => c.Name).HasMaxLength(100).IsRequired();
        b.Property(c => c.Slug).HasMaxLength(100).IsRequired();
    }
}

public class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
{
    public void Configure(EntityTypeBuilder<ForumPost> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Title).HasMaxLength(300).IsRequired();
        b.HasOne(p => p.Category).WithMany()
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Author).WithMany()
            .HasForeignKey(p => p.AuthorId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(p => p.Replies).WithOne(r => r.Post)
            .HasForeignKey(r => r.PostId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ForumReplyConfiguration : IEntityTypeConfiguration<ForumReply>
{
    public void Configure(EntityTypeBuilder<ForumReply> b)
    {
        b.HasKey(r => r.Id);
        b.HasOne(r => r.Author).WithMany()
            .HasForeignKey(r => r.AuthorId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.HasKey(n => n.Id);
        b.HasIndex(n => new { n.UserId, n.IsRead });
        b.Property(n => n.Title).HasMaxLength(200);
        b.Property(n => n.Type).HasMaxLength(50);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.HasKey(a => a.Id);
        b.HasIndex(a => a.CreatedAt);
        b.Property(a => a.Action).HasMaxLength(100).IsRequired();
        b.Property(a => a.Resource).HasMaxLength(100).IsRequired();
    }
}
