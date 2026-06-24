using HackZone.Application.Features.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
        => Ok(await mediator.Send(new GetDashboardStatsQuery(), ct));

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
        => Ok(await mediator.Send(new GetUsersQuery(), ct));

    [HttpPost("users/{userId}/ban")]
    public async Task<IActionResult> BanUser(Guid userId, [FromBody] BanRequest req, CancellationToken ct)
    {
        await mediator.Send(new BanUserCommand(userId), ct);
        return Ok(new { message = "Foydalanuvchi ban qilindi." });
    }
}

public record BanRequest(string Reason);
