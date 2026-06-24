using HackZone.Domain.Common;

namespace HackZone.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string Type { get; private set; } = "info";
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(Guid userId, string title, string message, string type = "info") =>
        new() { UserId = userId, Title = title, Message = message, Type = type };

    public void MarkRead() { IsRead = true; ReadAt = DateTime.UtcNow; SetUpdated(); }
}
