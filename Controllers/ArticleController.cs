using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;
using Portfolio.Services;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController(IArticleService articleService, TokenService tokenService, ILogger<Program> logger) : ControllerBase
{
  [HttpPost]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> CreateArticle([FromForm] ArticleDTO request)
  {

    var userIdFromJwt = await tokenService.GetInformationFromToken(Request.HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (userIdFromJwt == null)
    {
      logger.LogError("Erreur lors de la récupération de l'`id` utilisateur  : {@0}", userIdFromJwt);
      return StatusCode(StatusCodes.Status404NotFound, "Utilisateur introuvable");
    }

    if (request.Cover?.Length > 1 * 1024 * 1024)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }

    try
    {
      var article = await articleService.CreateArticleAsync(userIdFromJwt, request);
      if (article is null)
      {
        logger.LogWarning("Article déjà existant : {Title}", request.Title);
        return Conflict("Un article avec ce titre existe déjà.");
      }

      logger.LogInformation("Article '{Title}' créé avec succès.", request.Title);
      return CreatedAtAction(nameof(CreateArticle), new { id = article.Id }, article);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Erreur lors de la création de l'article '{Title}'.", request.Title);
      return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetOneArticle(Guid id)
  {
    var article = await articleService.FindArticleByIdAsync(id);
    if (article == null)
    {
      logger.LogInformation("Article avec l'id : `{id}` introuvable", id);
      return StatusCode(StatusCodes.Status404NotFound, "Article introuvable");
    }
    return Ok(article);
  }

  [HttpGet()]
  public async Task<IActionResult> GetAllPublishedArticles()
  {
    var articles = await articleService.GetAllPubishedArticlesAsync();
    return Ok(articles);
  }

  [HttpGet("admin")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  public async Task<IActionResult> GetAllArticles()
  {
    var articles = await articleService.GetAllArticlesAsync();
    return Ok(articles);
  }

  [HttpPatch("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateArticle(Guid id, [FromForm] UpdateArticleDTO request)
  {
    if (request.NewCoverFile?.Length > 1 * 1024 * 1024)
    {
      logger.LogError("La taille du fichier ne doit pas excéder 1MB.");
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }
    try
    {
      if (id != request.Id)
      {
        logger.LogError("ERREUR : Les id ne correspondent pas.\n ID requête : {@0}\n Id article BDD : {@1}", id, request.Id);
        return StatusCode(StatusCodes.Status400BadRequest, $"Les id de la requête ne correspondent pas.");
      }
      await articleService.UpdateArticleAsync(id, request);
      return Ok(request);

    }
    catch (Exception ex)
    {
      logger.LogError(ex.Message);
      return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    }
  }

  [HttpPatch("{id}/publish")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> TogglePublishArticle(Guid id, [FromBody] TogglePublishArticleRequest request)
  {
    var authorIdFromToken = await tokenService.GetInformationFromToken(Request.HttpContext, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
    if (authorIdFromToken == null)
    {
      logger.LogError("Impossible de récupérer l'ID de l'utilisateur depuis le token JWT.");
      return Unauthorized("Utilisateur non authentifié.");
    }

    logger.LogInformation("Tentative de mise à jour de l'article {ArticleId} par l'utilisateur {AuthorId}.", id, authorIdFromToken);

    try
    {
      var article = await articleService.TogglePublishArticleAsync(id, request.IsPublished, authorIdFromToken);
      if (article == null)
      {
        logger.LogWarning("Article {ArticleId} non trouvé ou utilisateur {AuthorId} non autorisé.", id, authorIdFromToken);
        return NotFound("Article non trouvé ou accès refusé.");
      }

      logger.LogInformation("Article {ArticleId} mis à jour avec succès. Statut de publication : {IsPublished}.", article.Id, article.IsPublished);
      return Ok(article);
    }
    catch (UnauthorizedAccessException)
    {
      logger.LogWarning("Utilisateur {AuthorId} non autorisé à modifier l'article {ArticleId}.", authorIdFromToken, id);
      return Forbid("Vous n'êtes pas autorisé à modifier cet article.");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Erreur lors de la mise à jour de l'article {ArticleId}.", id);
      return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue.");
    }
  }


  [HttpDelete("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> DeleteOneArticle(Guid id)
  {
    try
    {
      logger.LogInformation("Article '{0}' supprimé avec succès.", id);
      await articleService.DeleteArticleAsync(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      StatusCode(StatusCodes.Status500InternalServerError);
      return NotFound();
    }
  }

}

