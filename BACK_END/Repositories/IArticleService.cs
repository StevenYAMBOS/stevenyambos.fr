using Portfolio.Entities;

namespace Portfolio.Repositories
{
    public interface IArticles
    {
        Task<Article> CreateArticleAsync(Article request);
        Task<Article> UpdateArticleAsync(Article request);
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article?> FindArticleByIdAsync(Guid id);
        Task DeleteArticleAsync(Article request);
    }
}
