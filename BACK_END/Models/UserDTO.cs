using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Portfolio.Enums;

namespace Portfolio.Models;

public record class UserDTO
{
  public string Email { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public Role Role { get; set; }
  // public DateTime LastLogin { get; set; } = DateTime.UtcNow.AddDays(1);
};

public class RegisterRequest
{
  [EmailAddress]
  [Required]
  public string? Email { get; set; }

  [Required]
  public string? Username { get; set; }

  [Required]
  [PasswordPropertyText]
  public string? Password { get; set; }

  // public string? Role { get; set; }
};



public class LoginRequest
{
  [Required]
  [EmailAddress]
  public string? Email { get; set; }
  [Required]
  [PasswordPropertyText]
  public string? Password { get; set; }
}

public class AuthResponse
{
  public string? Username { get; set; }
  public string? Email { get; set; }
  public string? Token { get; set; }
}