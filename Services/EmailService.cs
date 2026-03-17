
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task<string> SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            var smtpServer = Environment.GetEnvironmentVariable("smtpServer");
            var port = int.Parse(Environment.GetEnvironmentVariable("smtpPort"));
            var from = Environment.GetEnvironmentVariable("smtpUser");
            var password = Environment.GetEnvironmentVariable("smtpPassword");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Your Name", from));
            email.To.Add(new MailboxAddress("To Name", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            logger.LogInformation("Destinataire : {0}", from);
            logger.LogInformation("Expéditeur : {0}", toEmail);
            logger.LogInformation("Objet : {0}", subject);
            logger.LogInformation("Email : {0}", body);

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return "Email envoyé avec succès !";
        }
        catch (Exception ex)
        {
            logger.LogInformation("Erreur lors de l'envoi du mail : {0}", ex.Message);
            return $"Erreur lors de l'envoi du mail : {ex.Message}";
        }
    }
}