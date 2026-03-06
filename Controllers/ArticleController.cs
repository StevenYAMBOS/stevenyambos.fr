using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController(IArticleService articleService, ILogger<Program> logger) : ControllerBase
{
  [HttpPost]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> CreateArticle([FromForm] ArticleDTO request)
  {
    if (request.Cover?.Length > 1 * 1024 * 1024)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }

    try
    {
      var article = await articleService.CreateArticleAsync(request);
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
  [Authorize(AuthenticationSchemes = "Bearer")]
  public async Task<IActionResult> GetAllArticles()
  {
    var articles = await articleService.GetArticlesAsync();
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

