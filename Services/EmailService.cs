
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public async Task<string> SendEmail(string toEmail, string subject, string body, string path)
    {
        try
        {
            var smtpServer = Environment.GetEnvironmentVariable("smtpServer");
            var port = int.Parse(Environment.GetEnvironmentVariable("smtpPort"));
            var from = Environment.GetEnvironmentVariable("smtpUser");
            var password = Environment.GetEnvironmentVariable("smtpPassword");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("stevenyambos.fr", from));
            email.To.Add(new MailboxAddress("Me", toEmail));
            email.Subject = subject;
            // email.Body = new TextPart("html") { Text = body };

            var builder = new BodyBuilder
            {
                // Set the plain-text version of the message text
                TextBody = body
            };

            // We may also want to attach a calendar event for Monica's party...
            builder.Attachments.Add(path);

            // Now we just need to set the message body and we're done
            email.Body = builder.ToMessageBody();

            /*             var attachment = new MimePart("image", "gif")
                        {
                            Content = new MimeContent(File.OpenRead(path)),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(path)
                        };

                        // now create the multipart/mixed container to hold the message text and the
                        // image attachment
                        var multipart = new Multipart("mixed")
                        {
                            body,
                            attachment
                        };

                        // now set the multipart/mixed as the message body
                        email.Body = multipart; */

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