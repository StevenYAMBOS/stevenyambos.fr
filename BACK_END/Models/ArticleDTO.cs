namespace Portfolio.Models;

public record class ArticleDTO
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public string Cover { get; set; } = string.Empty;
  public List<string> Categories { get; set; } = [];
  public List<string> Tags { get; set; } = [];
}