using Banking.Application.Interfaces;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Configuration;

using MimeKit;

namespace Banking.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {
        var senderName =
            _configuration["EmailSettings:SenderName"]!;

        var senderEmail =
            _configuration["EmailSettings:SenderEmail"]!;

        var smtpServer =
            _configuration["EmailSettings:SmtpServer"]!;

        var username =
            _configuration["EmailSettings:Username"]!;

        var password =
            _configuration["EmailSettings:Password"]!;

        var port = Convert.ToInt32(
            _configuration["EmailSettings:Port"]);

        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                senderName,
                senderEmail));

        email.To.Add(
            MailboxAddress.Parse(toEmail));

        email.Subject = subject;

        email.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        // ONLY for Development/Docker Testing
        smtp.ServerCertificateValidationCallback =
            (s, c, h, e) => true;

        await smtp.ConnectAsync(
            smtpServer,
            port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            username,
            password);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}