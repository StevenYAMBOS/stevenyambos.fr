using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
  public class ProfilService(AppDbContext context, ILogger<ProfilService> log) : IProfilService
  {
    public async Task<ApplicationUser?> FindUserByIdAsync(string id)
    {
      var user = await context.Users.FindAsync(id);
      return user;
    }

    public async Task<ApplicationUser> UpdateProfilAsync(UpdateProfilDTO request)
    {
      try
      {
        var user = await FindUserByIdAsync(request.Id) ?? throw new KeyNotFoundException("Utilisateur non trouvé");
        user.Id = user.Id;
        user.Email = request.Email ?? user.Email;
        user.UserName = request.Username ?? user.UserName;

        context.Users.Update(user);
        await context.SaveChangesAsync();
        log.LogInformation("[SERVICE] Informations utilisateur : {@0}", user);
        return user;
      }
      catch (KeyNotFoundException)
      {
        throw;
      }
      catch (Exception ex)
      {
        log.LogInformation("[SERVICE] Erreur : {@0}", ex);
        throw new InvalidOperationException("Une erreur est survenue lors de la mise à jour de l'utilisateur.", ex);
      }
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
