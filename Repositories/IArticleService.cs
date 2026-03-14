using Microsoft.AspNetCore.JsonPatch;
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllPubishedArticlesAsync();
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article?> FindArticleByIdAsync(Guid id);
        Task<Article?> CreateArticleAsync(string userId, ArticleDTO request);
        Task<Article> UpdateArticleAsync(Guid articleId, UpdateArticleDTO request);
        Task<Article> TogglePublishArticleAsync(Guid articleId, bool isPublished, string authorIdFromToken);
        Task DeleteArticleAsync(Guid id);
    }
}
