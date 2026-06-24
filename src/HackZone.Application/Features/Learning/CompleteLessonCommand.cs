using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Learning;

public record CompleteLessonCommand(Guid LessonId) : IRequest;

public class CompleteLessonHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<CompleteLessonCommand>
{
    public async Task Handle(CompleteLessonCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();
        var uid = currentUser.UserId.Value;

        var lesson = await uow.Lessons.Query()
            .Include(l => l.Course).ThenInclude(c => c.Lessons)
            .FirstOrDefaultAsync(l => l.Id == cmd.LessonId, ct)
            ?? throw new NotFoundException("Lesson", cmd.LessonId);

        var enrollment = await uow.Enrollments.Query()
            .FirstOrDefaultAsync(e => e.UserId == uid && e.CourseId == lesson.CourseId, ct)
            ?? throw new BusinessException("Avval kursga yoziling.");

        var alreadyDone = await uow.LessonProgresses.Query()
            .AnyAsync(p => p.UserId == uid && p.LessonId == cmd.LessonId, ct);

        if (!alreadyDone)
        {
            await uow.LessonProgresses.AddAsync(LessonProgress.Create(uid, cmd.LessonId), ct);

            var totalLessons = lesson.Course.Lessons.Count(l => l.IsPublished);
            var completedLessons = await uow.LessonProgresses.Query()
                .CountAsync(p => p.UserId == uid
                    && lesson.Course.Lessons.Select(l => l.Id).Contains(p.LessonId), ct) + 1;

            var progress = totalLessons > 0 ? (int)((double)completedLessons / totalLessons * 100) : 0;
            enrollment.UpdateProgress(progress);

            if (progress == 100)
            {
                var alreadyCert = await uow.Certificates.Query()
                    .AnyAsync(c => c.UserId == uid && c.CourseId == lesson.CourseId, ct);

                if (!alreadyCert)
                {
                    var cert = Certificate.Issue(uid, lesson.CourseId, lesson.Course.Title);
                    await uow.Certificates.AddAsync(cert, ct);

                    var user = await uow.Users.Query().FirstOrDefaultAsync(u => u.Id == uid, ct);
                    user?.AddReputation(50);
                }
            }

            await uow.SaveChangesAsync(ct);
        }
    }
}
