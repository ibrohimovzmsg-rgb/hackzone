namespace HackZone.Domain.Interfaces;

public interface ILabOrchestrator
{
    Task<(string ContainerId, string AccessUrl)> StartContainerAsync(string image, string userId, CancellationToken ct = default);
    Task StopContainerAsync(string containerId, CancellationToken ct = default);
    Task<bool> IsRunningAsync(string containerId, CancellationToken ct = default);
}
