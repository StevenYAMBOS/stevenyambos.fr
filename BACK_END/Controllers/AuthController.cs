using Portfolio.Entities;
using Portfolio.Models;
using Portfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Serilog;

namespace Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            Console.WriteLine("Point de terminaison d'inscription appelé pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                Console.WriteLine("Échec de l'inscription pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
                return BadRequest("User already exists.");
            }
            Console.WriteLine("Utilisateur inscrit avec succès : {0} à {1}", request.Username, DateTime.Now);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            Console.WriteLine("Point de terminaison de connexion appelé pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                Console.WriteLine("Échec de la connexion pour l'utilisateur : {0} à {1}", request.Username, DateTime.Now);
                return BadRequest("Invalid username or password.");
            }
            Console.WriteLine("Utilisateur connecté avec succès : {0} à {1}", request.Username, DateTime.Now);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            Console.WriteLine("Point de terminaison de rafraîchissement de jeton appelé pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                Console.WriteLine("Échec du rafraîchissement du jeton pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
                return Unauthorized("Invalid refresh token.");
            }
            Console.WriteLine("Jeton rafraîchi avec succès pour l'ID utilisateur : {0} à {1}", request.UserId, DateTime.Now);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            Console.WriteLine("Point de terminaison authentifié accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            Console.WriteLine("Point de terminaison admin accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }
    }
}
