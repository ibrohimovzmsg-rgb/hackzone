using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Learning;

public record GetLessonQuery(Guid LessonId) : IRequest<LessonDetail>;

public class GetLessonHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetLessonQuery, LessonDetail>
{
    public async Task<LessonDetail> Handle(GetLessonQuery q, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var lesson = await uow.Lessons.Query()
            .FirstOrDefaultAsync(l => l.Id == q.LessonId && l.IsPublished, ct)
            ?? throw new NotFoundException("Lesson", q.LessonId);

        var isCompleted = await uow.LessonProgresses.Query()
            .AnyAsync(p => p.UserId == currentUser.UserId.Value && p.LessonId == q.LessonId, ct);

        return new LessonDetail(lesson.Id, lesson.Title, lesson.Content,
            lesson.VideoUrl, lesson.OrderIndex, lesson.DurationMinutes, isCompleted);
    }
}
