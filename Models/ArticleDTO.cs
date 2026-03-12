using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models;

public record class ArticleDTO
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Guid Author { get; set; }
  public string Slug { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public bool IsPublished { get; set; }
  public IFormFile? Cover { get; set; }
  public List<string> Categories { get; set; } = [];
  public List<string> Tags { get; set; } = [];
}

public record class UpdateArticleDTO
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Guid Author { get; set; }
  public string Slug { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public string? Cover { get; set; }
  public bool IsPublished { get; set; }
  public IFormFile? NewCoverFile { get; set; }
  public List<string> Categories { get; set; } = [];
  public List<string> Tags { get; set; } = [];
}

public class IsPublishDTO
{
  public bool IsPublished { get; set; }
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class TogglePublishArticleRequest
{
  public bool IsPublished { get; set; }
}
