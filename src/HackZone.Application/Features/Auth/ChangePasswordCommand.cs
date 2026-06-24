using FluentValidation;
using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace HackZone.Application.Features.Auth;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Kamida 1 ta katta harf.")
            .Matches("[0-9]").WithMessage("Kamida 1 ta raqam.");
    }
}

public class ChangePasswordHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var user = await uow.Users.Query()
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
            ?? throw new NotFoundException("User", currentUser.UserId);

        if (!BC.Verify(cmd.CurrentPassword, user.PasswordHash))
            throw new BusinessException("Joriy parol noto'\''g'\''ri.");

        user.SetPasswordHash(BC.HashPassword(cmd.NewPassword));
        await uow.SaveChangesAsync(ct);
    }
}
