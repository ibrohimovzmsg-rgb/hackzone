using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Enums;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Labs;

public record StopLabCommand(Guid LabId) : IRequest;

public class StopLabHandler(IUnitOfWork uow, ICurrentUserService currentUser, ILabOrchestrator orchestrator)
    : IRequestHandler<StopLabCommand>
{
    public async Task Handle(StopLabCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var instance = await uow.LabInstances.Query()
            .FirstOrDefaultAsync(i => i.UserId == currentUser.UserId.Value
                && i.LabId == cmd.LabId && i.Status == LabStatus.Running, ct)
            ?? throw new NotFoundException("LabInstance", cmd.LabId);

        await orchestrator.StopContainerAsync(instance.ContainerId, ct);
        instance.Stop();
        await uow.SaveChangesAsync(ct);
    }
}
