
namespace Portfolio.Repositories;

public interface IEmailService
{
    Task SendEmail(string sender, string subject, string body, IFormFile file);
}