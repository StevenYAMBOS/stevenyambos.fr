
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmail(string sender, string subject, string body, IFormFile file)
    {
        try
        {
            var smtpServer = Environment.GetEnvironmentVariable("smtpServer");
            var port = int.Parse(Environment.GetEnvironmentVariable("smtpPort"));
            var receiver = Environment.GetEnvironmentVariable("smtpUser");
            var password = Environment.GetEnvironmentVariable("smtpPassword");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("stevenyambos.fr", sender));
            email.To.Add(new MailboxAddress("Destinataire", receiver));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = body
            };

            if (email.Attachments != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                builder.Attachments.Add(fileName, file.OpenReadStream());
            }
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(receiver, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Erreur lors de l'envoi du mail : {0}", ex.Message);
            // return $"Erreur lors de l'envoi du mail : {ex.Message}";
        }
    }
}