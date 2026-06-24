using Docker.DotNet;
using Docker.DotNet.Models;
using HackZone.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HackZone.Infrastructure.Services;

public class DockerLabOrchestrator(IConfiguration config, ILogger<DockerLabOrchestrator> logger) : ILabOrchestrator
{
    private DockerClient CreateClient()
    {
        var endpoint = config["Docker:Endpoint"] ?? "unix:///var/run/docker.sock";
        return new DockerClientConfiguration(new Uri(endpoint)).CreateClient();
    }

    public async Task<(string ContainerId, string AccessUrl)> StartContainerAsync(
        string image, string userId, CancellationToken ct = default)
    {
        using var client = CreateClient();

        try { await client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = image, Tag = "latest" },
            null, new Progress<JSONMessage>(), ct); }
        catch { /* image allaqachon bor */ }

        var port = new Random().Next(20000, 30000);
        var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            Labels = new Dictionary<string, string> { ["hackzone.userId"] = userId, ["hackzone.lab"] = "true" },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    ["80/tcp"] = [new PortBinding { HostPort = port.ToString() }]
                },
                Memory = 536870912, // 512MB
                NanoCPUs = 1000000000 // 1 CPU
            }
        }, ct);

        await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters(), ct);

        var vpsIp = config["Docker:VpsIp"] ?? "185.191.141.88";
        return (container.ID, $"http://{vpsIp}:{port}");
    }

    public async Task StopContainerAsync(string containerId, CancellationToken ct = default)
    {
        using var client = CreateClient();
        try
        {
            await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 5 }, ct);
            await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true }, ct);
        }
        catch (Exception ex) { logger.LogWarning("Container stop xato: {Error}", ex.Message); }
    }

    public async Task<bool> IsRunningAsync(string containerId, CancellationToken ct = default)
    {
        using var client = CreateClient();
        try
        {
            var inspect = await client.Containers.InspectContainerAsync(containerId, ct);
            return inspect.State.Running;
        }
        catch { return false; }
    }
}
