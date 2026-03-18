
namespace Portfolio.Repositories;

public interface IEmailService
{
    Task<string> SendEmail(string toEmail, string subject, string body, string path);
}