using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Entities;

[Table("user_articles")]
public class UserArticle
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [ForeignKey("users")]
    [Column("user_id")]
    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey("articles")]
    [Column("article_id")]
    public string? AID { get; set; }
    public virtual Article? Article { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}