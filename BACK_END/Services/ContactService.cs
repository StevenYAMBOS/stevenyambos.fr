

using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class ContactService(AppDbContext context, IFileService fileService, ILogger log) : IContactService
{
  public async Task<Contact> SendContactInfoAsync(SendContactInfoDTO request)
  {
    var contactId = Guid.NewGuid();
    string? fileUrl = null;
    string bucketFolder = "contacts";

    if (request.File is not null)
    {
      string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".webp", ".svg"];
      fileUrl = await fileService.UploadFileAsync(request.File, allowedExtensions, bucketFolder, contactId);
    }

    var contact = new Contact
    {
      Email = request.Email,
      Subject = request.Subject,
      Content = request.Content,
      File = fileUrl,
      CreatedAt = DateTime.UtcNow,
    };

    context.Contacts.Add(contact);
    await context.SaveChangesAsync();

    return contact;
  }
}