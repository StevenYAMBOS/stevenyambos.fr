using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article?> FindArticleByIdAsync(Guid id);
        Task<Article> CreateArticleAsync(ArticleDTO request);
        Task<Article> UpdateArticleAsync(Guid articleId, UpdateArticleDTO request);
        Task DeleteArticleAsync(Guid id);
    }
}
