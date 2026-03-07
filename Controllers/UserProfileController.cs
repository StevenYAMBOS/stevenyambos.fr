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

    var dataExtractedFromJwt = await tokenService.GetInformationFromToken(Request.HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (dataExtractedFromJwt == null)
    {
      logger.LogError("Erreur lors de la récupération de l'utilisateur  : {@0}", dataExtractedFromJwt);
      return StatusCode(StatusCodes.Status404NotFound, "Utilisateur introuvable");
    }

    logger.LogInformation("TOKEN décodé : {@0}", dataExtractedFromJwt);

    // var handler = new JwtSecurityTokenHandler();
    // string authHeader = Request.Headers.Authorization;
    // authHeader = authHeader.Replace("Bearer ", "");
    // var jsonToken = handler.ReadToken(authHeader);
    // var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
    // var id = tokenS.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

    var user = await userProfilService.FindUserByIdAsync(dataExtractedFromJwt);
    if (user == null)
    {
      logger.LogError("Utilisateur introuvable.");
      return StatusCode(StatusCodes.Status404NotFound, "Utilisateur introuvable");
    }

    logger.LogInformation("Informations utilisateur : {@0}", user);
    return Ok(user);
  }


  [HttpPatch()]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateUserProfil([FromBody] JsonPatchDocument<ApplicationUser> patchDocument)
  {
    if (patchDocument == null)
    {
      logger.LogError("La requête est nulle : {@0}", patchDocument);
      return BadRequest();
    }

    var dataExtractedFromJwt = await tokenService.GetInformationFromToken(Request.HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (dataExtractedFromJwt == null)
    {
      logger.LogError("Erreur lors de la récupération de l'utilisateur  : {@0}", dataExtractedFromJwt);
      return StatusCode(StatusCodes.Status404NotFound, "Utilisateur introuvable");
    }

    var (success, existingUser, error) = await userProfilService.UpdateProfilAsync(dataExtractedFromJwt, patchDocument);

    if (!success)
    {
      logger.LogError("Une erreur est sruvenue");
      return NotFound(new { error });
    }

    logger.LogInformation("Utilisateur mis à jour avec succès : {@0}", existingUser);
    return Ok(existingUser);
  }

  [HttpDelete()]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> DeleteUserProfile()
  {
    var dataExtractedFromJwt = await tokenService.GetInformationFromToken(Request.HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (dataExtractedFromJwt == null)
    {
      logger.LogError("Erreur lors de la récupération de l'utilisateur  : {@0}", dataExtractedFromJwt);
      return StatusCode(StatusCodes.Status404NotFound, "Utilisateur introuvable");
    }

    try
    {
      logger.LogInformation("Utilisateur '{0}' supprimé avec succès.", dataExtractedFromJwt);
      await userProfilService.DeleteProfilAsync(dataExtractedFromJwt);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      StatusCode(StatusCodes.Status500InternalServerError);
      return NotFound();
    }
  }
}

