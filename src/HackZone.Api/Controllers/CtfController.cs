using HackZone.Application.Features.Ctf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/ctf")]
public class CtfController(IMediator mediator) : ControllerBase
{
    [HttpGet("challenges")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetChallengesQuery(), ct));

    [Authorize]
    [HttpGet("challenges/{id}")]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
        => Ok(await mediator.Send(new GetChallengeDetailQuery(id), ct));

    [Authorize]
    [HttpPost("challenges/{id}/submit")]
    public async Task<IActionResult> SubmitFlag(Guid id, [FromBody] SubmitFlagRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new SubmitFlagCommand(id, req.Flag), ct));

    [HttpGet("leaderboard")]
    public async Task<IActionResult> Leaderboard(CancellationToken ct)
        => Ok(await mediator.Send(new GetLeaderboardQuery(), ct));
}

public record SubmitFlagRequest(string Flag);
