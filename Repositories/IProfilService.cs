using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories;

public interface IProfilService
{
  Task<ApplicationUser?> FindUserByIdAsync(string id);
  Task<ApplicationUser> UpdateProfilAsync(UpdateProfilDTO request);
  Task DeleteProfilAsync(string id);
}