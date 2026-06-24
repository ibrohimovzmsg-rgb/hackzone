using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Enums;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Labs;

public record StartLabCommand(Guid LabId) : IRequest<StartLabResponse>;

public class StartLabHandler(IUnitOfWork uow, ICurrentUserService currentUser, ILabOrchestrator orchestrator)
    : IRequestHandler<StartLabCommand, StartLabResponse>
{
    public async Task<StartLabResponse> Handle(StartLabCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();
        var uid = currentUser.UserId.Value;

        var lab = await uow.Labs.Query()
            .FirstOrDefaultAsync(l => l.Id == cmd.LabId && l.IsPublished, ct)
            ?? throw new NotFoundException("Lab", cmd.LabId);

        var existing = await uow.LabInstances.Query()
            .FirstOrDefaultAsync(i => i.UserId == uid && i.LabId == cmd.LabId
                && i.Status == LabStatus.Running && !i.IsExpired, ct);

        if (existing != null)
            return new StartLabResponse(existing.ContainerId, existing.AccessUrl!, existing.ExpiresAt);

        var (containerId, accessUrl) = await orchestrator.StartContainerAsync(
            lab.DockerImage, uid.ToString(), ct);

        var instance = LabInstance.Create(uid, lab.Id, containerId, accessUrl, lab.ExpiryMinutes);
        await uow.LabInstances.AddAsync(instance, ct);
        await uow.SaveChangesAsync(ct);

        return new StartLabResponse(containerId, accessUrl, instance.ExpiresAt);
    }
}
