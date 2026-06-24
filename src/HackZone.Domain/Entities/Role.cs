using HackZone.Domain.Common;

namespace HackZone.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() { }

    public static Role Create(string name, string description) =>
        new() { Name = name, Description = description };

    public static class Names
    {
        public const string Student = "Student";
        public const string Instructor = "Instructor";
        public const string Admin = "Admin";
    }
}

public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = default!;
    public User User { get; private set; } = default!;

    private UserRole() { }

    public static UserRole Create(Guid userId, Guid roleId) =>
        new() { UserId = userId, RoleId = roleId };
}
