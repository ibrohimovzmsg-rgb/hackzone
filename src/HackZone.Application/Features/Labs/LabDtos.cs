namespace HackZone.Application.Features.Labs;

public record LabListItem(Guid Id, string Title, string Description,
    string Difficulty, string Category, bool IsRunning, string? AccessUrl, DateTime? ExpiresAt);

public record StartLabResponse(string ContainerId, string AccessUrl, DateTime ExpiresAt);
