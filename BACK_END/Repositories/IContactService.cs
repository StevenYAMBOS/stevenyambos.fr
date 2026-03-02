
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories;

public interface IContactService
{
  Task<Contact> SendContactInfoAsync(SendContactInfoDTO request);
  // Task<Contact> GetContactAsync(SendContactInfoDTO request);
}