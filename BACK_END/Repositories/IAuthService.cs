using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Repositories
{
    public interface IAuthService
    {
        Task<ApplicationUser?> RegisterAsync(RegisterRequest request);
        /*         Task<TokenResponseDTO?> LoginAsync(UserDTO request);
                Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request); */
    }
}
