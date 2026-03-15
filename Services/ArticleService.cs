
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Services
{
    public class ArticleService(AppDbContext context, IFileService fileService, IUserProfileService userService, ILogger<ArticleService> log) : IArticleService
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
                .Replace("?", "")
                .Replace("!", "")
                .Trim('-');
        }

        public async Task<IEnumerable<Article>> GetAllPubishedArticlesAsync()
        {
            var articles = await context.Articles
            .FromSqlRaw("SELECT * FROM articles WHERE is_published=true ORDER BY created_at ASC")
            .AsNoTracking()
            .ToListAsync();
            // var articles = await context.Articles.ToListAsync();
            return articles;
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            var articles = await context.Articles.ToListAsync();
            return articles;
        }

        public async Task<Article?> CreateArticleAsync(string userId, ArticleDTO request)
        {
            if (await context.Articles.AnyAsync(a => a.Title == request.Title))
            {
                return null;
            }

            var user = await userService.FindUserByIdAsync(userId) ?? throw new KeyNotFoundException("Utilisateur non trouvé");

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
                IsPublished = false,
                Cover = coverUrl,
                Categories = request.Categories,
                Tags = request.Tags,
                AuthorId = user.Id,
                PublishedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // var userArticle = new UserArticle
            // {
            //     UserId = user.Id,
            //     AID = articleId.ToString(),
            //     CreatedAt = DateTime.UtcNow,
            // };

            context.Articles.Add(article);
            // context.UserArticleTable.Add(userArticle);
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
                existingArticle.Slug = GenerateSlug(request?.Title) ?? existingArticle.Slug;
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

        public async Task<Article> TogglePublishArticleAsync(Guid articleId, bool isPublished, string authorIdFromToken)
        {
            try
            {
                var existingArticle = await FindArticleByIdAsync(articleId);
                if (existingArticle == null)
                {
                    log.LogWarning("Article {ArticleId} non trouvé.", articleId);
                    return null;
                }

                // if (existingArticle.Author.ToString() != authorIdFromToken)
                // {
                //     log.LogWarning("Utilisateur {AuthorId} non autorisé à modifier l'article {ArticleId}.", authorIdFromToken, articleId);
                //     throw new UnauthorizedAccessException();
                // }

                existingArticle.IsPublished = isPublished;
                existingArticle.PublishedAt = isPublished ? DateTime.UtcNow : null;
                existingArticle.UpdatedAt = DateTime.UtcNow;

                context.Articles.Update(existingArticle);
                await context.SaveChangesAsync();

                log.LogInformation("Article {ArticleId} mis à jour. Statut de publication : {IsPublished}.", existingArticle.Id, existingArticle.IsPublished);
                return existingArticle;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Erreur lors de la mise à jour de l'article {ArticleId}.", articleId);
                throw;
            }
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            var article = await FindArticleByIdAsync(articleId);
            if (article == null)
                throw new KeyNotFoundException("Article non trouvé");
            else
            {
                await fileService.DeleteFileAsync(article?.Cover.Replace("https://pub-56d2c024e16e477e9fe29e4b168d78ec.r2.dev/", ""));
                context.Articles.Remove(article);
                await context.SaveChangesAsync();
            }
        }

    }
}