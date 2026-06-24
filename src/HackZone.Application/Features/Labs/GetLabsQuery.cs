using HackZone.Domain.Entities;
using HackZone.Domain.Enums;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Labs;

public record GetLabsQuery : IRequest<List<LabListItem>>;

public class GetLabsHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetLabsQuery, List<LabListItem>>
{
    public async Task<List<LabListItem>> Handle(GetLabsQuery _, CancellationToken ct)
    {
        var uid = currentUser.UserId;
        var labs = await uow.Labs.Query()
            .Where(l => l.IsPublished)
            .OrderBy(l => l.Difficulty)
            .ToListAsync(ct);

        List<LabInstance> instances = [];
        if (uid.HasValue)
        {
            instances = await uow.LabInstances.Query()
                .Where(i => i.UserId == uid.Value && i.Status == LabStatus.Running)
                .ToListAsync(ct);
        }

        return labs.Select(lab =>
        {
            var inst = instances.FirstOrDefault(i => i.LabId == lab.Id && !i.IsExpired);
            return new LabListItem(
                lab.Id, lab.Title, lab.Description,
                lab.Difficulty.ToString(), lab.Category.ToString(),
                inst != null, inst?.AccessUrl, inst?.ExpiresAt);
        }).ToList();
    }
}
