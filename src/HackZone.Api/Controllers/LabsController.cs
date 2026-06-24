using HackZone.Application.Features.Labs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/labs")]
[Authorize]
public class LabsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await mediator.Send(new GetLabsQuery(), ct));

    [HttpPost("{labId}/start")]
    public async Task<IActionResult> Start(Guid labId, CancellationToken ct)
        => Ok(await mediator.Send(new StartLabCommand(labId), ct));

    [HttpPost("instances/{instanceId}/stop")]
    public async Task<IActionResult> Stop(Guid instanceId, CancellationToken ct)
    {
        await mediator.Send(new StopLabCommand(instanceId), ct);
        return Ok(new { message = "Lab to'\''xtatildi." });
    }
}
