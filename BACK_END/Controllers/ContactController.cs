using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Enums;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(IContactService contactService, ILogger<Program> log) : ControllerBase
{
  [HttpPost]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> SendContactForm([FromForm] SendContactInfoDTO request)
  {
    if (request.File?.Length > 1 * 1024 * 1024)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }

    try
    {
      var contactInfos = await contactService.SendContactInfoAsync(request);
      if (contactInfos is null)
      {
        log.LogWarning("Erreur lors de l'envoie du formulaire : {Subject}", request.Subject);
        return Conflict("Erreur lors de l'envoie du formulaire.");
      }

      log.LogInformation("Formulaire envoyé avec succès.");
      return CreatedAtAction(nameof(SendContactForm), new { id = contactInfos.Id }, contactInfos);
    }
    catch (Exception ex)
    {
      log.LogError(ex, "Erreur lors de l'envoi du formulaire '{Subject}'.", request.Subject);
      return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
    }
  }

  /*   [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
    public async Task<IActionResult> GetOneArticle(Guid id)
    {
      var contactInfos = await contactService.FindArticleByIdAsync(id);
      if (contactInfos == null)
      {
        log.LogInformation("contactInfos avec l'id : `{id}` introuvable", id);
        return StatusCode(StatusCodes.Status404NotFound, "contactInfos introuvable");
      }
      return Ok(contactInfos);
    }

    [HttpGet()]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
    public async Task<IActionResult> GetAllArticles()
    {
      var articles = await contactService.GetArticlesAsync();
      return Ok(articles);
    }
   */
}

