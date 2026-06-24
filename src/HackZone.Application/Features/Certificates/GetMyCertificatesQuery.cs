using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Certificates;

public record CertificateItem(Guid Id, string CourseTitle, string Code, DateTime IssuedAt);

public record GetMyCertificatesQuery : IRequest<List<CertificateItem>>;

public class GetMyCertificatesHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetMyCertificatesQuery, List<CertificateItem>>
{
    public async Task<List<CertificateItem>> Handle(GetMyCertificatesQuery _, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var certs = await uow.Certificates.Query()
            .Where(c => c.UserId == currentUser.UserId.Value)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync(ct);

        return certs.Select(c => new CertificateItem(c.Id, c.CourseTitle, c.Code, c.IssuedAt)).ToList();
    }
}
