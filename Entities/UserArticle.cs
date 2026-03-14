using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Entities;

[Table("user_articles")]
public class UserArticle
{
    [Column("user_id")]
    [Key]
    public Guid UserId { get; set; }

    [Column("article_id")]
    public Guid ArticleId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}