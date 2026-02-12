using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IArticles
    {
        Task<Article> CreateArticleAsync(ArticleDTO request);
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article?> FindArticleByIdAsync(Guid id);
        Task DeleteArticleAsync(ArticleDTO request);
    }
}
