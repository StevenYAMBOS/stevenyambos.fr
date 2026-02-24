
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
    public class ArticleService(ApplicationDbContext context, IFileService fileService) : IArticleService
    {
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

        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            var articles = await context.Articles.ToListAsync();
            return articles;
        }

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

        public async Task<Article?> FindArticleByIdAsync(Guid id)
        {
            var article = await context.Articles.FindAsync(id);
            return article;
        }

        public async Task<Article> UpdateArticleAsync(Guid id, UpdateArticleDTO request)
        {
            try
            {
                var existingArticle = await FindArticleByIdAsync(id) ?? throw new KeyNotFoundException("Article non trouvé");
                string oldImage = existingArticle?.Cover;
                if (request.NewCoverFile != null)
                {
                    var articleId = Guid.NewGuid();
                    string? coverUrl = null;
                    string bucketFolder = "articles";
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".webp", ".svg"];
                    coverUrl = await fileService.UploadFileAsync(request.NewCoverFile, allowedExtensions, bucketFolder, articleId);
                    request.Cover = coverUrl;
                }

                existingArticle.Cover = request.NewCoverFile != null ? request.Cover : existingArticle.Cover;
                existingArticle.Id = existingArticle.Id;
                existingArticle.Title = request.Title ?? existingArticle.Title;
                existingArticle.Slug = request.Slug ?? existingArticle.Slug;
                existingArticle.Description = request.Description ?? existingArticle.Description;
                existingArticle.Content = request.Content ?? existingArticle.Content;
                existingArticle.Categories = request.Categories ?? existingArticle.Categories;
                existingArticle.Tags = request.Tags ?? existingArticle.Tags;
                existingArticle.UpdatedAt = DateTime.UtcNow;

                context.Articles.Update(existingArticle);
                await context.SaveChangesAsync();

                if (request.NewCoverFile != null && oldImage != null)
                    await fileService.DeleteFileAsync(oldImage.Replace("https://pub-56d2c024e16e477e9fe29e4b168d78ec.r2.dev/", ""));

                return existingArticle;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Une erreur est survenue lors de la mise à jour de l'article.", ex);
            }
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            var article = await FindArticleByIdAsync(articleId);
            if (article == null)
                throw new KeyNotFoundException("Article non trouvé");
            else
            {
                await fileService.DeleteFileAsync(article.Cover.Replace("https://pub-56d2c024e16e477e9fe29e4b168d78ec.r2.dev/", ""));
                context.Articles.Remove(article);
                await context.SaveChangesAsync();
            }
        }
    }
}
