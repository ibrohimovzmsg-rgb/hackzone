using HackZone.Application.Features.Certificates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/certificates")]
[Authorize]
public class CertificatesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await mediator.Send(new GetMyCertificatesQuery(), ct));
}
