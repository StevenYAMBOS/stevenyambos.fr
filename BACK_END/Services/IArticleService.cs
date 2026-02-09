using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Articles
{
  public interface IArticles
  {
    Task<Article?> CreateArticleAsync(ArticleDTO request);
    Task<Article?> GetOneArticleAsync(ArticleDTO request);
  }
}