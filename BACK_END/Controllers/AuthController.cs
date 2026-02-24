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
        ApplicationDbContext context,
         UserManager<ApplicationUser> userManager,
         TokenService tokenService,
         SignInManager<ApplicationUser> signInManager,
         //  AuthService authService,
         ILogger<Program> log
         ) : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // Assign default role
                await userManager.AddToRoleAsync(user, "Admin");
                return Ok(new { message = "Registration successful" });
            }
            return BadRequest(result.Errors);

            /*             try
                        {
                            var user = await authService.RegisterAsync(request);
                            if (user is null)
                            {
                                log.LogWarning("Utilisateur déjà existant : {0}", request.Username);
                                return Conflict("Un utilisateur avec ce nom existe déjà.");
                            }

                            log.LogInformation("Utilisateur '{0}' créé avec succès.", request.Username);
                            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
                        }
                        catch (Exception ex)
                        {
                            log.LogError(ex, "Erreur lors de la création de l'utilisateur '{0}'.", request.Username);
                            return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur interne est survenue.");
                        } */

            /*
                         var result = await userManager.CreateAsync(
                            new ApplicationUser { UserName = request.Username, Email = request.Email, Role = Role.User },
                            request.Password!
                        );

                        if (result.Succeeded)
                        {
                            request.Password = "";
                            return CreatedAtAction(nameof(Register), new { email = request.Email, role = request.Role }, request);
                        }

                        return Ok(result); */
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDTO request)
        {
            var user = await userManager.FindByEmailAsync(request.Email!);
            if (user == null)
            {
                return BadRequest("Identifiants incorrects");
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password!);
            if (!isPasswordValid)
            {
                return BadRequest("Identifiants incorrects");
            }

            var existingUser = context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (existingUser is null)
            {
                return Unauthorized();
            }

            var accessToken = tokenService.CreateToken(existingUser);
            await context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                Username = existingUser.UserName,
                Email = existingUser.Email,
                Token = accessToken,
            });
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
        // [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            Console.WriteLine("✅ UTILISATEUR : {0} à {1}", User.Identity?.Name, DateTime.Now);
            log.LogInformation("Point de terminaison authentifié accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            Console.WriteLine("✅ UTILISATEUR ADMIN : {0} à {1}", User.Identity?.Name, DateTime.Now);
            log.LogInformation("✅ Point de terminaison admin accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }
    }
}
