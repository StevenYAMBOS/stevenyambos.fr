namespace Portfolio.Models;

public record class ArticleDTO
{
  public string Title { get; set; } = string.Empty;
  public string Slug { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public IFormFile? Cover { get; set; }
  public List<string> Categories { get; set; } = [];
  public List<string> Tags { get; set; } = [];
}