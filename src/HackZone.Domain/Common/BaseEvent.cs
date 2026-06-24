namespace HackZone.Domain.Common;

public abstract record BaseEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
