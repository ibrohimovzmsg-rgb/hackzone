using HackZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackZone.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> b)
    {
        b.HasKey(c => c.Id);
        b.HasIndex(c => c.Slug).IsUnique();
        b.Property(c => c.Title).HasMaxLength(200).IsRequired();
        b.Property(c => c.Slug).HasMaxLength(200).IsRequired();
        b.Property(c => c.Description).HasMaxLength(2000);
        b.Property(c => c.Category).HasMaxLength(100);

        b.HasMany(c => c.Lessons).WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(c => c.Enrollments).WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> b)
    {
        b.HasKey(l => l.Id);
        b.Property(l => l.Title).HasMaxLength(200).IsRequired();
        b.Property(l => l.Content).IsRequired();
    }
}

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> b)
    {
        b.HasKey(e => e.Id);
        b.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
    }
}

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> b)
    {
        b.HasKey(p => p.Id);
        b.HasIndex(p => new { p.UserId, p.LessonId }).IsUnique();
        b.HasOne(p => p.Lesson).WithMany()
            .HasForeignKey(p => p.LessonId).OnDelete(DeleteBehavior.Cascade);
    }
}
