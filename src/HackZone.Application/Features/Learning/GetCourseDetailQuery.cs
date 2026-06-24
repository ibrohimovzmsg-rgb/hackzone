using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Learning;

public record GetCourseDetailQuery(string Slug) : IRequest<CourseDetail>;

public class GetCourseDetailHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetCourseDetailQuery, CourseDetail>
{
    public async Task<CourseDetail> Handle(GetCourseDetailQuery q, CancellationToken ct)
    {
        var course = await uow.Courses.Query()
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Slug == q.Slug && c.IsPublished, ct)
            ?? throw new NotFoundException("Course", q.Slug);

        var userId = currentUser.UserId;
        Enrollment? enrollment = null;
        List<LessonProgress> progress = [];

        if (userId.HasValue)
        {
            enrollment = await uow.Enrollments.Query()
                .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.CourseId == course.Id, ct);

            if (enrollment != null)
            {
                var lessonIds = course.Lessons.Select(l => l.Id).ToList();
                progress = await uow.LessonProgresses.Query()
                    .Where(p => p.UserId == userId.Value && lessonIds.Contains(p.LessonId))
                    .ToListAsync(ct);
            }
        }

        var completedIds = progress.Select(p => p.LessonId).ToHashSet();
        var lessons = course.Lessons
            .Where(l => l.IsPublished)
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonItem(l.Id, l.Title, l.OrderIndex, l.DurationMinutes, completedIds.Contains(l.Id)))
            .ToList();

        return new CourseDetail(
            course.Id, course.Title, course.Description,
            course.Difficulty.ToString(), course.Category, course.ThumbnailUrl,
            lessons, enrollment != null, enrollment?.ProgressPercent ?? 0);
    }
}
