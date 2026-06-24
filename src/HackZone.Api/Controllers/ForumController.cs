using HackZone.Application.Features.Forum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/forum")]
public class ForumController(IMediator mediator) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
        => Ok(await mediator.Send(new GetCategoriesQuery(), ct));

    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts([FromQuery] Guid? categoryId, CancellationToken ct)
        => Ok(await mediator.Send(new GetPostsQuery(categoryId), ct));

    [HttpGet("posts/{id}")]
    public async Task<IActionResult> GetPost(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetPostDetailQuery(id), ct));

    [Authorize]
    [HttpPost("posts")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new CreatePostCommand(req.CategoryId, req.Title, req.Content), ct));

    [Authorize]
    [HttpPost("posts/{postId}/replies")]
    public async Task<IActionResult> CreateReply(Guid postId, [FromBody] CreateReplyRequest req, CancellationToken ct)
    {
        await mediator.Send(new CreateReplyCommand(postId, req.Content), ct);
        return Ok(new { message = "Javob qo'shildi." });
    }
}

public record CreatePostRequest(Guid CategoryId, string Title, string Content, string? Type = null);
public record CreateReplyRequest(string Content);
