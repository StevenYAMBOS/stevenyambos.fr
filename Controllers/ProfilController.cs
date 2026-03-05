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
public class ProfilController(AppDbContext context, IProfilService profilService, ILogger<ProfilController> logger) : ControllerBase
{
  [HttpPatch("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateProfil(string id, [FromBody] JsonPatchDocument<ApplicationUser> patchDocument)
  {
    if (patchDocument == null)
    {
      return BadRequest();
    }
    var existingUser = await profilService.UpdateProfilAsync(id, patchDocument);
    if (existingUser == null)
    {
      return NotFound();
    }
    patchDocument.ApplyTo(existingUser, ModelState);

    if (!TryValidateModel(existingUser))
    {
      return BadRequest(ModelState);
    }
    context.Users.Update(existingUser);
    await context.SaveChangesAsync();

    return Ok(existingUser);
    /*     logger.LogInformation("Requête : {@0}", patchDocument);
        if (id == null)
        {
          logger.LogError("ID manquant : {@0}", id);
          return NotFound();
        }

        var updatedUser = await profilService.UpdateProfilAsync(id, patchDocument);

        logger.LogWarning("Profil utilisateur mis à jour : {0}", updatedUser);
        return Ok(updatedUser); */
    /*     logger.LogInformation("Requête : {@0}", patchDocument);
        if (patchDocument == null)
        {
          logger.LogError("patchDocument est nul : {@0}", patchDocument);
          return BadRequest();
        }

        var existingUser = context.Users.FirstOrDefault(user => user.Id == id);
        if (existingUser == null)
        {
          logger.LogInformation("ID manquant : {@0}", id);
          return NotFound();
        }

        patchDocument.ApplyTo(existingUser);
        logger.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
        return Ok(existingUser); */
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

