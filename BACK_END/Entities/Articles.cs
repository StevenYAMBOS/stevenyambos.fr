using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Entities;

[Table("articles")]
public class Article
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("title")]
  [Required]
  [MaxLength(200)]
  public string Title { get; set; } = string.Empty;

  [Column("slug")]
  [Required]
  [MaxLength(250)]
  public string Slug { get; set; } = string.Empty;

  [Column("description")]
  [MaxLength(500)]
  public string Description { get; set; } = string.Empty;

  [Column("content")]
  [Required]
  public string Content { get; set; } = string.Empty;

  [Column("cover")]
  [MaxLength(500)]
  public string? Cover { get; set; }

  [Column("author")]
  public Guid Author { get; set; }

  [Column("is_published")]
  public bool IsPublished { get; set; } = false;

  [Column("published_at")]
  public DateTime? PublishedAt { get; set; }

  [Column("categories")]
  public List<string> Categories { get; set; } = [];

  [Column("tags")]
  public List<string> Tags { get; set; } = [];

  [Column("view_count")]
  public int ViewCount { get; set; } = 0;

  [Column("reading_time_minutes")]
  public int ReadingTimeMinutes { get; set; } = 0;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}