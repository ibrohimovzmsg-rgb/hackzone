using HackZone.Domain.Common;
using HackZone.Domain.Enums;

namespace HackZone.Domain.Entities;

public class Lab : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string DockerImage { get; private set; } = default!;
    public Difficulty Difficulty { get; private set; }
    public ChallengeCategory Category { get; private set; }
    public bool IsPublished { get; private set; }
    public int ExpiryMinutes { get; private set; } = 120;

    private Lab() { }

    public static Lab Create(string title, string description, string dockerImage,
        Difficulty difficulty, ChallengeCategory category) => new()
    {
        Title = title, Description = description, DockerImage = dockerImage,
        Difficulty = difficulty, Category = category
    };

    public void Publish() { IsPublished = true; SetUpdated(); }
}

public class LabInstance : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid LabId { get; private set; }
    public string ContainerId { get; private set; } = default!;
    public LabStatus Status { get; private set; } = LabStatus.Stopped;
    public string? AccessUrl { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? StoppedAt { get; private set; }
    public Lab Lab { get; private set; } = default!;

    private LabInstance() { }

    public static LabInstance Create(Guid userId, Guid labId, string containerId, string accessUrl, int expiryMinutes) => new()
    {
        UserId = userId, LabId = labId, ContainerId = containerId,
        Status = LabStatus.Running, AccessUrl = accessUrl,
        StartedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
    };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Stop()
    {
        Status = LabStatus.Stopped;
        StoppedAt = DateTime.UtcNow;
        SetUpdated();
    }
}
