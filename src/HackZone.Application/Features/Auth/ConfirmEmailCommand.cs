using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Auth;

public record ConfirmEmailCommand(string Token) : IRequest;

public class ConfirmEmailHandler(IUnitOfWork uow) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand cmd, CancellationToken ct)
    {
        var user = await uow.Users.Query()
            .FirstOrDefaultAsync(u => u.EmailConfirmationToken == cmd.Token, ct)
            ?? throw new NotFoundException("Token", cmd.Token);

        user.ConfirmEmail();
        await uow.SaveChangesAsync(ct);
    }
}
