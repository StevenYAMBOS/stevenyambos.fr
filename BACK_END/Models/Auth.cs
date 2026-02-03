namespace Portfolio.Models;

public record class Register
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