using System.Net;
using System.Net.Mail;
using Directory.Application.Interfaces;
using Directory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Directory.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IApplicationDbContext _db;

    public EmailService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string tempPassword)
    {
        var settings = await _db.SystemSettings.FirstOrDefaultAsync();
        if (settings == null)
            throw new InvalidOperationException("System settings not configured.");

        using var client = new SmtpClient(settings.MailServer, settings.ServerPort)
        {
            EnableSsl = settings.RequireSsl
        };

        if (settings.RequireAuth)
        {
            client.Credentials = new NetworkCredential(settings.SmtpUser, settings.SmtpPassword);
        }

        var mail = new MailMessage
        {
            From = new MailAddress(settings.FromEmail, settings.FromUsername),
            IsBodyHtml = true,
            Subject = "Password Reset Confirmation",
            Body = $"Your new password has been set to: {tempPassword}<br /><br />" +
                   $"Please login to change your password to something you can remember.<br /><br />" +
                   $"Thanks,<br />{settings.FromUsername}"
        };
        mail.To.Add(new MailAddress(toEmail));

        await client.SendMailAsync(mail);
    }
}
