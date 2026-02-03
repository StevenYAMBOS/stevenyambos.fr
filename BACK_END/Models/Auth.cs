namespace Portfolio.Models;

public record class Register
(
  string Email,
  string Username,
  string Password
  );

public record class Login
(
  string Email,
  string Username,
  string Password
  );