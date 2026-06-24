namespace HackZone.Domain.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string username, string token, CancellationToken ct = default);
    Task SendPasswordResetAsync(string toEmail, string username, string token, CancellationToken ct = default);
}
