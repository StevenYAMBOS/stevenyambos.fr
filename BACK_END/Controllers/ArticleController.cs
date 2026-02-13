using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;
using Portfolio.Services;

namespace Portfolio.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ArticleController(IFileService fileService, IArticleService articleService, ILogger<Program> log) : ControllerBase
  {
    [HttpPost]
    public async Task<IActionResult> CreateArticle([FromForm] ArticleDTO request)
    {
      try
      {
        var article = await articleService.CreateArticleAsync(request);
        if (request.Cover?.Length > 1 * 1024 * 1024)
        {
          return StatusCode(StatusCodes.Status400BadRequest, "La taille du fichier ne doit pas excédder 1MB.");
        }
        if (article is null)
        {
          log.LogError("Erreur lors de la création de l'article : {0} à {1}", request.Title, DateTime.Now);
          return BadRequest("L'utilisateur existe déjà.");
        }
        log.LogInformation("{Title} créé avec succès.", request.Title);
        return Ok(article);

        // string[] allowedExtensions = [".jpeg", ".png", ".webp", ".svg"];
        // string filePath = "/articles";
        // string createdImageName = await fileService.SaveFileAsync(request.Cover, allowedExtensions, filePath, request.Id);

        // var article = new Article();
        // {
        //   article.Title = request.Title,
        //   article.Slug = request.Slug,
        //   article.Description = request.Description,
        //   article.Content = request.Content,
        //   article.Content = request.Content,
        //   article.Author = request.Author,
        //   article.Categories = request.Categories,
        //   article.Tags = request.Tags,
        // };

        // var createdArticle = await articleService.CreateArticleAsync(article);
        // return CreatedAtAction(nameof(CreateArticle), createdArticle);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }
    }
  }
}
