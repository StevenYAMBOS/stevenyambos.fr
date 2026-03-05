using Microsoft.AspNetCore.JsonPatch;
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories;

public interface IProfilService
{
  Task<ApplicationUser?> FindUserByIdAsync(string id);
  Task<ApplicationUser> UpdateProfilAsync(string id, JsonPatchDocument<ApplicationUser> patchDocument);
  Task DeleteProfilAsync(string id);
}