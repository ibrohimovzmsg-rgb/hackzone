using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Learning;

public record GetCoursesQuery : IRequest<List<CourseListItem>>;

public class GetCoursesHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetCoursesQuery, List<CourseListItem>>
{
    public async Task<List<CourseListItem>> Handle(GetCoursesQuery _, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var enrollments = userId.HasValue
            ? await uow.Enrollments.Query()
                .Where(e => e.UserId == userId.Value)
                .ToListAsync(ct)
            : [];

        var courses = await uow.Courses.Query()
            .Include(c => c.Lessons)
            .Where(c => c.IsPublished)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync(ct);

        return courses.Select(c =>
        {
            var enrollment = enrollments.FirstOrDefault(e => e.CourseId == c.Id);
            return new CourseListItem(
                c.Id, c.Title, c.Description, c.Slug,
                c.Difficulty.ToString(), c.Category, c.ThumbnailUrl,
                c.Lessons.Count(l => l.IsPublished),
                enrollment != null,
                enrollment?.ProgressPercent ?? 0);
        }).ToList();
    }
}
