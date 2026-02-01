public class Article
{
  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Description { get; set; }
  public string? Content { get; set; }
  public bool IsPublished { get; set; }
  public string? Categories { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}