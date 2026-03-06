using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Repositories;
using Portfolio.Services;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/user/profile")]
public class UserProfileController(TokenService tokenService, IUserProfileService userProfilService, ILogger<UserProfileController> logger) : ControllerBase
{

  [HttpGet()]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> GetUserProfil()
  {

    /*     var dataExtractedFromJwt = await tokenService.GetInformationFromToken(context, "Id");
        if (dataExtractedFromJwt == null)
        {
          logger.LogError("Informations token utilisateur : {@0}", dataExtractedFromJwt);
          return StatusCode(StatusCodes.Status404NotFound, "Article introuvable");
        } */

    var handler = new JwtSecurityTokenHandler();
    string authHeader = Request.Headers["Authorization"];
    authHeader = authHeader.Replace("Bearer ", "");
    // logger.LogError("Informations authHeader : {@0}", authHeader);
    var jsonToken = handler.ReadToken(authHeader);
    var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
    logger.LogError("Informations tokenS : {@0}", JsonConvert.SerializeObject(tokenS.Claims.FirstOrDefault(claim => claim.Type == "Value"), Formatting.Indented));
    var id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
    logger.LogError("Informations token utilisateur : {@0}", id);

    return Ok(id);
  }


  [HttpPatch("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateUserProfil(string id, [FromBody] JsonPatchDocument<ApplicationUser> patchDocument)
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

