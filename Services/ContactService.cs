

using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class ContactService(AppDbContext context, IFileService fileService, ILogger<ContactService> logger) : IContactService
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

  public async Task<Contact?> FindContactByIdAsync(Guid id)
  {
    var contact = await context.Contacts.FindAsync(id);
    logger.LogInformation("Information de la demande : {0}", contact);
    return contact;
  }

  public async Task<IEnumerable<Contact>> GetContactsAsync()
  {
    var contacts = await context.Contacts.ToListAsync();
    logger.LogInformation("LISTE DES DEMANDES : {@0}", contacts);
    return contacts;
  }
}