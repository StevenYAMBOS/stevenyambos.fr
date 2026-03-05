using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
  public class ProfilService(AppDbContext context, ILogger<ProfilService> logger) : IProfilService
  {
    public async Task<ApplicationUser?> FindUserByIdAsync(string id)
    {
      var user = await context.Users.FindAsync(id);
      return user;
    }

    public async Task<ApplicationUser> UpdateProfilAsync(string id, JsonPatchDocument<ApplicationUser> patchDocument)
    {
      var existingUser = context.Users.FirstOrDefault(user => user.Id == id);
      if (existingUser == null)
      {
        logger.LogWarning("Erreur lors de la mise à jour de l'utilisateur.");
        return null;
      }

      logger.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
      patchDocument.ApplyTo(existingUser);

      return existingUser;
    }

    public async Task DeleteProfilAsync(string id)
    {
      var user = await FindUserByIdAsync(id);
      if (user == null)
        throw new KeyNotFoundException("Utilisateur non trouvé");
      else
      {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
      }
    }
  }
}
