
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
    public class ArticleService(AppDbContext context) : IArticles
    {
        public async Task<Article> CreateArticleAsync(Article request)
        {
            context.Articles.Add(request);
            await context.SaveChangesAsync();

            return request;
        }

        public async Task DeleteArticleAsync(Article request)
        {
            context.Articles.Remove(request);
            await context.SaveChangesAsync();
        }

        public async Task<Article?> FindArticleByIdAsync(Guid id)
        {
            var article = await context.Articles.FindAsync(id);
            return article;
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            var articles = await context.Articles.ToListAsync();
            return articles;
        }

        public async Task<Article> UpdateArticleAsync(Article request)
        {
            context.Articles.Update(request);
            await context.SaveChangesAsync();
            return request;
        }
    }
}
