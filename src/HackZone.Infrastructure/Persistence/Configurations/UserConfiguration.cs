using HackZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackZone.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(u => u.Id);
        b.HasIndex(u => u.Email).IsUnique();
        b.HasIndex(u => u.Username).IsUnique();
        b.Property(u => u.Email).HasMaxLength(256).IsRequired();
        b.Property(u => u.Username).HasMaxLength(50).IsRequired();
        b.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        b.Property(u => u.DisplayName).HasMaxLength(100);
        b.Property(u => u.Bio).HasMaxLength(500);
        b.Property(u => u.Country).HasMaxLength(100);

        b.HasMany(u => u.UserRoles).WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(u => u.RefreshTokens).WithOne()
            .HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.HasKey(r => r.Id);
        b.HasIndex(r => r.Name).IsUnique();
        b.Property(r => r.Name).HasMaxLength(50).IsRequired();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b.HasKey(ur => new { ur.UserId, ur.RoleId });
        b.HasOne(ur => ur.Role).WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.HasKey(rt => rt.Id);
        b.HasIndex(rt => rt.Token).IsUnique();
        b.Property(rt => rt.Token).HasMaxLength(500).IsRequired();
    }
}
