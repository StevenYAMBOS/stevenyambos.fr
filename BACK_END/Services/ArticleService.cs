
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Articles
{
  public class ArticleService(AppDbContext context) : IArticles
  {
    public async Task<Article?> CreateArticleAsync(ArticleDTO request)
    {
      if (await context.Articles.AnyAsync(a => a.Title == request.Title))
      {
        return null;
      }

      var article = new Article
      {
        Title = request.Title,
        Slug = request.Slug,
        Description = request.Description,
        Content = request.Content,
        // Cover = request.Cover,
        Categories = request.Categories,
        Tags = request.Tags,
        CreatedAt = DateTime.UtcNow.AddDays(1)
      };
      context.Articles.Add(article);
      await context.SaveChangesAsync();

      return article;
    }

    public Task<Article?> GetOneArticleAsync(ArticleDTO request)
    {
      throw new NotImplementedException();
    }
  }
}