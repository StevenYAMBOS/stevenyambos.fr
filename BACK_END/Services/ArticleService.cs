
using System.Globalization;
using System.Text;
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

            var articleId = Guid.NewGuid();
            string? coverUrl = null;
            string bucketFolder = "articles";

            if (request.Cover is not null)
            {
                string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".webp", ".svg"];
                coverUrl = await fileService.UploadFileAsync(request.Cover, allowedExtensions, bucketFolder, articleId);
            }

            var slug = GenerateSlug(request.Title);

            var article = new Article
            {
                Title = request.Title,
                Slug = slug,
                Description = request.Description,
                Content = request.Content,
                Cover = coverUrl,
                Author = request.Author,
                Categories = request.Categories,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Articles.Add(article);
            await context.SaveChangesAsync();

            return article;
        }

        private static string GenerateSlug(string title)
        {
            return title
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace(" ", "-")
                .Replace("'", "-")
                .Replace("--", "-")
                .Trim('-');
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
