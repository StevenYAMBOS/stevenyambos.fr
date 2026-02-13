
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
    public class ArticleService(AppDbContext context, IFileService fileService) : IArticleService
    {
        public async Task<Article> CreateArticleAsync(ArticleDTO request)
        {
            if (await context.Articles.AnyAsync(a => a.Title == request.Title))
            {
                return null;
            }
            string[] allowedExtensions = [".jpeg", ".png", ".webp", ".svg"];
            string filePath = "/articles";
            await fileService.SaveFileAsync(request.Cover, allowedExtensions, filePath, request.Id);
            var article = new Article
            {
                Title = request.Title,
                Slug = request.Slug,
                Description = request.Description,
                Content = request.Content,
                // article.Cover = createdImageName;
                Author = request.Author,
                Categories = request.Categories,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow.AddDays(1)
            };
            context.Articles.Add(article);
            await context.SaveChangesAsync();

            return article;
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
