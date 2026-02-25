using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, IEnumerable<string>? Errors)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, TokenResponseDTO? Tokens, string? Error)> LoginAsync(LoginRequest request);
        Task<(bool Success, TokenResponseDTO? Tokens, string? Error)> RefreshTokensAsync(RefreshTokenRequestDTO request);
    }
}
