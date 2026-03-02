

using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class ContactService(AppDbContext context, IFileService fileService) : IContactService
{
  public async Task<Contact> SendContactInfoAsync(SendContactInfoDTO request)
  {
    var contactId = Guid.NewGuid();
    Console.WriteLine("NEW ID FORM : {0}", contactId);
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

    Console.WriteLine("REQUÊTE EMAIL : {0}", request.Email);
    Console.WriteLine("REQUÊTE SUBJECT : {0}", request.Subject);
    Console.WriteLine("REQUÊTE CONTENT : {0}", request.Content);
    Console.WriteLine("REQUÊTE FILE : {0}", fileUrl);

    context.Contacts.Add(contact);
    await context.SaveChangesAsync();

    return contact;
  }
}