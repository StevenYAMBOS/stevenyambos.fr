using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilController(AppDbContext context, IProfilService profilService, ILogger<ProfilController> log) : ControllerBase
{
  [HttpPatch("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateProfil(string id, [FromBody] JsonPatchDocument<ApplicationUser> patchDoc)
  {
    log.LogInformation("Requête : {@0}", patchDoc);
    if (patchDoc == null)
    {
    log.LogInformation("PatchDoc est nul : {@0}", patchDoc);
      return BadRequest();
    }

    var existingUser = context.Users.FirstOrDefault(user => user.Id == id);
    if (existingUser == null)
    {
    log.LogInformation("ID manquant : {@0}", id);
      return NotFound();
    }

  // ⚠️ EN GROS : Ajouter l'appel en base de données mon ami !
  patchDoc.ApplyTo(existingUser);
/*     context.Users.Update(existingUser);
    await context.SaveChangesAsync(); */
    log.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
    return Ok(existingUser);
  }


/*   [HttpPut()]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateProfil([FromBody] UpdateProfilDTO request)
  {
    try
    {
      if (request.Id == null)
      {
        log.LogError("[CONTROLLEUR] Id introuvable : {@0}");
        return StatusCode(StatusCodes.Status400BadRequest, $"Id introuvable.");
      }
      await profilService.UpdateProfilAsync(request);
      log.LogInformation("[CONTROLLEUR] Informations utilisateur : {@0}", request);
      return Ok(request);
    }
    catch (Exception ex)
    {
      log.LogError(ex.Message);
      return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    }
  } */
}

