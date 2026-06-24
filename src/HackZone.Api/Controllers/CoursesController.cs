using HackZone.Application.Features.Learning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetCoursesQuery(), ct));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetDetail(string slug, CancellationToken ct)
        => Ok(await mediator.Send(new GetCourseDetailQuery(slug), ct));

    [Authorize]
    [HttpPost("{courseId}/enroll")]
    public async Task<IActionResult> Enroll(Guid courseId, CancellationToken ct)
    {
        await mediator.Send(new EnrollCommand(courseId), ct);
        return Ok(new { message = "Kursga yozilindi." });
    }

    [Authorize]
    [HttpGet("{courseId}/lessons/{lessonId}")]
    public async Task<IActionResult> GetLesson(Guid courseId, Guid lessonId, CancellationToken ct)
        => Ok(await mediator.Send(new GetLessonQuery(lessonId), ct));

    [Authorize]
    [HttpPost("{courseId}/lessons/{lessonId}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid courseId, Guid lessonId, CancellationToken ct)
    {
        await mediator.Send(new CompleteLessonCommand(lessonId), ct);
        return Ok(new { message = "Dars bajarildi." });
    }
}
