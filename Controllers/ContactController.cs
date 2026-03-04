using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Enums;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(IContactService contactService, ILogger<ContactController> log) : ControllerBase
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

  [HttpGet("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
  public async Task<IActionResult> GetContact(Guid id)
  {
    var contact = await contactService.FindContactByIdAsync(id);
    if (contact == null)
    {
      log.LogInformation("Demande avec l'id : `{id}` introuvable", id);
      return StatusCode(StatusCodes.Status404NotFound, "contact introuvable");
    }
    return Ok(contact);
  }

  [HttpGet("all")]
  [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
  public async Task<IActionResult> GetAllContacts()
  {
    var contacts = await contactService.GetContactsAsync();
    return Ok(contacts);
  }


  [HttpDelete("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> DeleteOneContact(Guid id)
  {
    try
    {
      log.LogInformation("Article '{0}' supprimé avec succès.", id);
      await contactService.DeleteContatAsync(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      StatusCode(StatusCodes.Status500InternalServerError);
      return NotFound();
    }
  }
}

