using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

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