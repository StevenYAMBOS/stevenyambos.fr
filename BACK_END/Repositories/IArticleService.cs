using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IArticles
    {
        Task<Article?> CreateArticleAsync(ArticleDTO request);
        Task<Article?> GetOneArticleAsync(ArticleDTO request);
    }
}
