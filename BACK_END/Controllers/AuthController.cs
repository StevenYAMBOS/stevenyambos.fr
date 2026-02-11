using Portfolio.Entities;
using Portfolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Repositories;

namespace Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<Program> log) : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                log.LogError("Échec de l'inscription pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
                return BadRequest("L'utilisateur existe déjà.");
            }
            log.LogInformation("{Username} inscrit avec succès.", request.Username);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                log.LogError("Échec de la connexion pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
                return BadRequest("Email ou mot de passe incorrect.");
            }
            log.LogInformation("{Username} connecté avec succès.", request.Username);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            log.LogInformation("Point de terminaison de rafraîchissement de jeton appelé pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                log.LogInformation("Échec du rafraîchissement du jeton pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
                return Unauthorized("Invalid refresh token.");
            }
            log.LogInformation("Jeton rafraîchi avec succès pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            log.LogInformation("Point de terminaison authentifié accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            log.LogInformation("Point de terminaison admin accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }
    }
}
