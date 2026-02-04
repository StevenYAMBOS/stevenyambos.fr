namespace Portfolio.Entities;

public class User
{
  public int Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool IsAdmin { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}