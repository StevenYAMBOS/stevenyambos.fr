namespace Portfolio.Models;

public record class RegisterModel
(
  string Email,
  string Username,
  string Password
  );

public record class LoginModel
(
  string Email,
  string Username,
  string Password
  );