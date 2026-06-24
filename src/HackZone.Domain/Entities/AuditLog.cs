using HackZone.Domain.Common;

namespace HackZone.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = default!;
    public string Resource { get; private set; } = default!;
    public string? ResourceId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? Details { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(Guid? userId, string action, string resource,
        string? resourceId = null, string? ip = null, string? details = null) => new()
    {
        UserId = userId, Action = action, Resource = resource,
        ResourceId = resourceId, IpAddress = ip, Details = details
    };
}
