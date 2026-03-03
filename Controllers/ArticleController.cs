using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;
using Portfolio.Services;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController(IArticleService articleService, ILogger<Program> log) : ControllerBase
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
        log.LogWarning("Article déjà existant : {Title}", request.Title);
        return Conflict("Un article avec ce titre existe déjà.");
      }

      log.LogInformation("Article '{Title}' créé avec succès.", request.Title);
      return CreatedAtAction(nameof(CreateArticle), new { id = article.Id }, article);
    }
    catch (Exception ex)
    {
      log.LogError(ex, "Erreur lors de la création de l'article '{Title}'.", request.Title);
      return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetOneArticle(Guid id)
  {
    var article = await articleService.FindArticleByIdAsync(id);
    if (article == null)
    {
      log.LogInformation("Article avec l'id : `{id}` introuvable", id);
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

  [HttpPut("{id}")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [EnableRateLimiting("fixed")]
  public async Task<IActionResult> UpdateArticle(Guid id, [FromForm] UpdateArticleDTO request)
  {
    if (request.NewCoverFile?.Length > 1 * 1024 * 1024)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excéder 1MB.");
    }
    try
    {
      if (id != request.Id)
      {
        return StatusCode(StatusCodes.Status400BadRequest, $"Les `id` du `body` et de la requête ne correspondent pas pour cette article");
      }
      await articleService.UpdateArticleAsync(id, request);
      return Ok(request);

    }
    catch (Exception ex)
    {
      log.LogError(ex.Message);
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
      log.LogInformation("Article '{0}' supprimé avec succès.", id);
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

