using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Enums;
using Portfolio.Models;
using Portfolio.Repositories;

namespace Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IAuthService authService,
        ILogger<AuthController> logger
    ) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, token, errors) = await authService.RegisterAsync(request);

            if (!success)
            {
                logger.LogWarning("Échec de l'inscription pour {Email}", request.Email);
                return BadRequest(new { errors });
            }

            return CreatedAtAction(nameof(Register), new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, tokens, error) = await authService.LoginAsync(request);

            if (!success)
            {
                logger.LogWarning("Échec de connexion pour {Email}", request.Email);
                return Unauthorized(new { error });
            }

            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, tokens, error) = await authService.RefreshTokensAsync(request);

            if (!success)
                return Unauthorized(new { error });

            return Ok(tokens);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AuthenticatedOnly()
        {
            var username = User.Identity?.Name;
            logger.LogInformation("✅ Endpoint authentifié accessible par {User}", username);
            return Ok(new { message = "Vous êtes authentifié.", user = username });
        }

        [HttpGet("admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
        public IActionResult AdminOnly()
        {
            var username = User.Identity?.Name;
            logger.LogInformation("✅ Endpoint admin accessible par {User}", username);
            return Ok(new { message = "Vous êtes Admin.", user = username });
        }
    }
}