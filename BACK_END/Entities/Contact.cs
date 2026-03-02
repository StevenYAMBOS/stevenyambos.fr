
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Entities;

[Table("contacts")]
public class Contact
{
  [Column("id")]
  public Guid Id { get; set; }

  [Column("email", TypeName = "varchar(100)")]
  [Required]
  [MaxLength(200)]
  [EmailAddress]
  public string? Email { get; set; }

  [Column("subject")]
  [Required]
  [MaxLength(200)]
  public string? Subject { get; set; }

  [Column("content")]
  [Required]
  [MaxLength(1000)]
  public string? Content { get; set; }

  [Column("file", TypeName = "varchar(100)")]
  [MaxLength(200)]
  public string? File { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}