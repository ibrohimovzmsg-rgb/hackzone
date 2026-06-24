using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Learning;

public record EnrollCommand(Guid CourseId) : IRequest;

public class EnrollHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<EnrollCommand>
{
    public async Task Handle(EnrollCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var course = await uow.Courses.Query()
            .FirstOrDefaultAsync(c => c.Id == cmd.CourseId && c.IsPublished, ct)
            ?? throw new NotFoundException("Course", cmd.CourseId);

        var exists = await uow.Enrollments.Query()
            .AnyAsync(e => e.UserId == currentUser.UserId.Value && e.CourseId == cmd.CourseId, ct);

        if (exists) throw new ConflictException("Siz bu kursga allaqachon yozilgansiz.");

        var enrollment = Enrollment.Create(currentUser.UserId.Value, cmd.CourseId);
        await uow.Enrollments.AddAsync(enrollment, ct);
        await uow.SaveChangesAsync(ct);
    }
}
