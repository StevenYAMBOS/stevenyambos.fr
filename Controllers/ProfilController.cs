using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilController(IProfilService profilService, ILogger<ProfilController> log) : ControllerBase
{
  [HttpPut()]
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
  }
}

