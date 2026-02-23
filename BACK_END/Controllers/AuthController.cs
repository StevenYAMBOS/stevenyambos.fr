using Portfolio.Entities;
using Portfolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Repositories;
using Portfolio.Services;
using Microsoft.AspNetCore.Identity;
using Portfolio.Enums;
using Portfolio.Data;

namespace Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        AppDbContext context,
         UserManager<UserRoles> userManager,
         TokenService tokenService,
         SignInManager<UserRoles> signInManager,
         //  IAuthService authService,
         ILogger<Program> log
         ) : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            var result = await userManager.CreateAsync(
                new UserRoles { UserName = request.Username, Email = request.Email, Role = Role.User },
                request.Password!
            );

            if (result.Succeeded)
            {
                request.Password = "";
                return CreatedAtAction(nameof(Register), new { email = request.Email, role = request.Role }, request);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDTO request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }
            var result = await signInManager.CheckPasswordSignInAsync(
                user, request.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var token = tokenService.CreateToken(user);
                return Ok(new { token });
            }
            return BadRequest("Invalid credentials");
        }

        /*         [HttpPost("refresh-token")]
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
         */
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            log.LogInformation("Point de terminaison authentifié accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            log.LogInformation("Point de terminaison admin accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }
    }
}
