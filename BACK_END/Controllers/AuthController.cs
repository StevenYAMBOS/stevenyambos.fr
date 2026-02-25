using Portfolio.Entities;
using Portfolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Repositories;
using Portfolio.Services;
using Microsoft.AspNetCore.Identity;
using Portfolio.Enums;
using Portfolio.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        AppDbContext context,
         UserManager<IdentityUser> userManager,
         RoleManager<ApplicationUser> roleManager,
         SignInManager<IdentityUser> signInManager,
         TokenService tokenService,
         IConfiguration configuration,
         //  AuthService authService,
         ILogger<Program> log
         ) : ControllerBase
    {
        public static User user = new User();

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Instantiate a identity user that is needed to authenticate
                // using the data from InboundUser
                var user = new ApplicationUser { UserName = request.Email, Email = request.Email };

                // Here, we search if a role of type "User" already exists
                // The role will define which type of user has the token and
                // its permissions
                bool userRoleExists = await roleManager.RoleExistsAsync(Role.User.ToString());

                // In case of role of type User doesn't exists, create one
                if (!userRoleExists)
                {
                    await roleManager.CreateAsync(new ApplicationUser { Role = Role.User });
                }

                // Now, we create the user
                var result = await userManager.CreateAsync(user, request.Password);
                // And set the role of "User" to it
                await userManager.AddToRoleAsync(user, Role.User.ToString());

                var errors = result.Errors.Select(e => e.Description);
                // In case of success, build the token
                // Otherwise, just return the errors
                if (result.Succeeded)
                {
                    var token = tokenService.CreateToken(user);
                    if (token == null)
                    {
                        return BadRequest("Email or password invalid!");
                    }
                    return Ok(token);
                }
                else
                {
                    return BadRequest(errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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

        private async Task<string> BuildToken(RegisterRequest request, Role[] roleTypes)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var claims = new List<Claim>() {
              new Claim(JwtRegisteredClaimNames.Email, request.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roleTypes)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

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

        [HttpGet("admin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = nameof(Role.Admin))]
        public IActionResult AdminOnly()
        {
            Console.WriteLine("✅ UTILISATEUR ADMIN : {0} à {1}", User.Identity?.Name, DateTime.Now);
            log.LogInformation("✅ Point de terminaison admin accessible par l'utilisateur : {0} à {1}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }
    }
}
