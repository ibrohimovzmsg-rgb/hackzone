using HackZone.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackZone.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new RegisterCommand(req.Username, req.Email, req.Password), ct));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new LoginCommand(req.Email, req.Password), ct));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new RefreshTokenCommand(req.RefreshToken), ct));

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken ct)
    {
        await mediator.Send(new ConfirmEmailCommand(token), ct);
        return Ok(new { message = "Email tasdiqlandi." });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
        => Ok(await mediator.Send(new GetCurrentUserQuery(), ct));

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req, CancellationToken ct)
        => Ok(await mediator.Send(new UpdateProfileCommand(req.DisplayName, req.Bio, req.Country), ct));

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        await mediator.Send(new ChangePasswordCommand(req.CurrentPassword, req.NewPassword), ct);
        return Ok(new { message = "Parol o'zgartirildi." });
    }
}
