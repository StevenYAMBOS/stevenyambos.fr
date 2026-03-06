using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/user/profile")]
public class UserProfileController(IUserProfileService userProfilService, ILogger<UserProfileController> logger) : ControllerBase
{
  [HttpPatch("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateProfil(string id, [FromBody] JsonPatchDocument<ApplicationUser> patchDocument)
  {
    if (patchDocument == null)
    {
      logger.LogError("La requête est nulle : {@0}", patchDocument);
      return BadRequest();
    }

    var (success, existingUser, error) = await userProfilService.UpdateProfilAsync(id, patchDocument);

    if (!success)
    {
      logger.LogError("Une erreur est sruvenue");
      return NotFound(new { error });
    }

    logger.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
    return Ok(existingUser);
  }

  [HttpDelete("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> DeleteUserProfile(string id)
  {
    try
    {
      logger.LogInformation("Utilisateur '{0}' supprimé avec succès.", id);
      await userProfilService.DeleteProfilAsync(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      StatusCode(StatusCodes.Status500InternalServerError);
      return NotFound();
    }
  }
}

