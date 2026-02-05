using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Entities;

[Table("users")]
public class User
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  [Column("id")]
  public int Id { get; set; }
  [Column("email")]
  public string Email { get; set; } = string.Empty;
  [Column("username")]
  public string Username { get; set; } = string.Empty;
  [Column("password")]
  public string Password { get; set; } = string.Empty;
  [Column("is_admin")]
  public bool IsAdmin { get; set; } = false;
  [Column("refresh_token")]
  public string? RefreshToken { get; set; }
  [Column("refresh_token_expiry_time")]
  public DateTime? RefreshTokenExpiryTime { get; set; }
  [Column("created_at")]
  public DateTime? CreatedAt { get; set; }
  // public DateTime? UpdatedAt { get; set; }
}