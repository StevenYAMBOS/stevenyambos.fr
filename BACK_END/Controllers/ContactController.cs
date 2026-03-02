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
    Console.WriteLine("✅ Requête reçu : {0} !", request.Email);
    if (request.File?.Length > 1 * 1024 * 1024)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }

    try
    {
      var formrequest = await contactService.SendContactInfoAsync(request);
      if (formrequest is null)
      {
        Console.WriteLine("Erreur lors de l'envoie du formulaire : {Subject}", request.Subject);
        log.LogWarning("Erreur lors de l'envoie du formulaire : {Subject}", request.Subject);
        return Conflict("Erreur lors de l'envoie du formulaire.");
      }

      Console.WriteLine("Formulaire envoyé avec succès.");
      log.LogInformation("Formulaire envoyé avec succès.");
      return CreatedAtAction(nameof(SendContactForm), new { id = formrequest.Id }, formrequest);
    }
    catch (Exception ex)
    {
      Console.WriteLine("Erreur lors de l'envoi du formulaire '{Subject}'.", request.Subject);
      log.LogError(ex, "Erreur lors de l'envoi du formulaire '{Subject}'.", request.Subject);
      return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
    }
  }

  /*   [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
    public async Task<IActionResult> GetOneArticle(Guid id)
    {
      var formrequest = await contactService.FindArticleByIdAsync(id);
      if (formrequest == null)
      {
        log.LogInformation("formrequest avec l'id : `{id}` introuvable", id);
        return StatusCode(StatusCodes.Status404NotFound, "formrequest introuvable");
      }
      return Ok(formrequest);
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

