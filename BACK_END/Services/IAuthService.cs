using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Services
{
  public interface IAuthService
  {
    Task<User?> RegisterAsync(UserDTO request);
    Task<TokenResponseDTO?> LoginAsync(UserDTO request);
    Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
  }
}