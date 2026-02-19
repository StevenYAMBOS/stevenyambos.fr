using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(ArticleDTO request);
        Task<Article> UpdateArticleAsync(Article request);
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article?> FindArticleByIdAsync(Guid id);
        Task DeleteArticleAsync(Article article);
    }
}
