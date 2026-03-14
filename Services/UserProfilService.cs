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
  public class ProfilService(AppDbContext context, IFileService fileService, ILogger<ProfilService> logger) : IUserProfileService
  {
    public async Task<ApplicationUser?> FindUserByIdAsync(string id)
    {
      var user = await context.Users.FindAsync(id);
      return user;
    }

    public async Task<(bool Success, ApplicationUser? User, string? Error)> UpdateProfilAsync(string id, JsonPatchDocument<ApplicationUser> patchDocument)
    {
      try
      {
        var existingUser = context.Users.FirstOrDefault(user => user.Id == id);
        if (existingUser == null)
        {
          logger.LogWarning("Erreur lors de la mise à jour de l'utilisateur.");
          return (false, null, "Erreur lors de la mise à jour de l'utilisateur.");
        }

        logger.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
        patchDocument.ApplyTo(existingUser);

        context.Users.Update(existingUser);
        await context.SaveChangesAsync();

        return (true, existingUser, null);
      }
      catch (Exception ex)
      {
        logger.LogInformation("Erreur : {@0}", ex);
        throw new InvalidOperationException("Une erreur est survenue lors de la mise à jour de l'utilisateur.", ex);
      }
    }

    public async Task DeleteProfilAsync(string id)
    {
      var user = await FindUserByIdAsync(id) ?? throw new KeyNotFoundException("Utilisateur non trouvé");

      var articles = await context.Articles
        .Where(a => a.AuthorId == id)
        .ToListAsync();

      foreach (var article in articles)
      {
        if (article.Cover != null)
          await fileService.DeleteFileAsync(article.Cover.Replace("https://pub-56d2c024e16e477e9fe29e4b168d78ec.r2.dev/", ""));
      }

      context.Users.Remove(user);
      await context.SaveChangesAsync();
    }
  }
}
