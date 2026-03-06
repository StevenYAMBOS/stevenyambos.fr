using Microsoft.AspNetCore.JsonPatch;
using Portfolio.Entities;
namespace Portfolio.Repositories;

public interface IUserProfileService
{
  Task<ApplicationUser?> FindUserByIdAsync(string id);
  Task<(bool Success, ApplicationUser? User, string? Error)> UpdateProfilAsync(string id, JsonPatchDocument<ApplicationUser> patchDocument);
  Task DeleteProfilAsync(string id);
}