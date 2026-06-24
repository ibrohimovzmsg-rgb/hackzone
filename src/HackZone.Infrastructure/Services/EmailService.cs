using System.Net;
using System.Net.Mail;
using HackZone.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HackZone.Infrastructure.Services;

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendConfirmationEmailAsync(string toEmail, string username, string token, CancellationToken ct = default)
    {
        var baseUrl = config["App:BaseUrl"] ?? "https://hackzone.uz";
        var link = $"{baseUrl}/confirm-email?token={token}";
        var body = $@"
<h2>Salom, {username}!</h2>
<p>HackZone Cyber Academy ga xush kelibsiz.</p>
<p>Email manzilingizni tasdiqlash uchun quyidagi havolani bosing:</p>
<a href=""{link}"" style=""background:#DC2626;color:#fff;padding:12px 24px;text-decoration:none;border-radius:6px;"">
  Emailni tasdiqlash
</a>
<p>Havola 24 soat davomida amal qiladi.</p>";

        await SendAsync(toEmail, "HackZone — Emailni tasdiqlang", body, ct);
    }

    public async Task SendPasswordResetAsync(string toEmail, string username, string token, CancellationToken ct = default)
    {
        var baseUrl = config["App:BaseUrl"] ?? "https://hackzone.uz";
        var link = $"{baseUrl}/reset-password?token={token}";
        var body = $@"
<h2>Salom, {username}!</h2>
<p>Parolni tiklash uchun quyidagi havolani bosing:</p>
<a href=""{link}"">Parolni tiklash</a>
<p>Havola 1 soat davomida amal qiladi. Agar siz so'\''rov yubormagan bo'\''lsangiz, bu xabarni e'\''tiborsiz qoldiring.</p>";

        await SendAsync(toEmail, "HackZone — Parolni tiklash", body, ct);
    }

    private async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        try
        {
            var host = config["Email:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(config["Email:Port"], out var p) ? p : 587;
            var user = config["Email:Username"] ?? "";
            var pass = config["Email:Password"] ?? "";
            var from = config["Email:FromAddress"] ?? "noreply@hackzone.uz";
            var fromName = config["Email:FromName"] ?? "HackZone";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass)
            };

            var msg = new MailMessage
            {
                From = new MailAddress(from, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(to);
            await client.SendMailAsync(msg, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Email yuborishda xato: {Error}", ex.Message);
        }
    }
}
