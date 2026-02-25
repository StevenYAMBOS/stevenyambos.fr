using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Portfolio.Repositories;
using Portfolio.Enums;

namespace Portfolio.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager) : IAuthService
    {
        public async Task<ApplicationUser?> RegisterAsync(RegisterRequest request)
        {

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
            };
            var result = await userManager.CreateAsync(user, request.Password!);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "user");
            }
            return user;
            // if (await context.Users.AnyAsync(u => u.Email == request.Email))
            // {
            //     return null;
            // }
            // var user = new User();
            // var hashedPassword = new PasswordHasher<User>()
            //     .HashPassword(user, request.Password);

            // user.Email = request.Email;
            // user.Username = request.Username;
            // user.Password = hashedPassword;
            // user.Role = request.Role;
            // user.CreatedAt = DateTime.UtcNow.AddDays(1);
            // context.Users.Add(user);
            // await context.SaveChangesAsync();

            // return user;
        }

        /*         public async Task<TokenResponseDTO?> LoginAsync(UserDTO request)
                {
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => u.Email == request.Email);
                    if (user is null)
                    {
                        return null;
                    }

                    var passwordVerificationResult = new PasswordHasher<User>()
                        .VerifyHashedPassword(user, request.Password, request.Password);
                    user.LastLogin = DateTime.UtcNow.AddDays(1);
                    if (passwordVerificationResult == PasswordVerificationResult.Failed)
                    {
                        return null;
                    }

                    return await CreateTokenResponse(user);
                }

                private async Task<TokenResponseDTO> CreateTokenResponse(User user)
                {
                    return new TokenResponseDTO
                    {
                        AccessToken = CreateToken(user),
                        RefreshToken = await GenerateAndSaveRefreshToken(user)
                    };
                }

                private string CreateToken(User user)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                    var tokenDescriptor = new JwtSecurityToken(
                        issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                        audience: configuration.GetValue<string>("AppSettings:Audience"),
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: creds
                        );

                    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                }

                private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
                {
                    var user = await context.Users.FindAsync(userId);
                    if (user is null ||
                        user.RefreshToken != refreshToken ||
                        user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    {
                        return null;
                    }
                    return user;
                }

                private string GenerateRefreshToken()
                {
                    var randomNumber = new byte[32];
                    using var rng = RandomNumberGenerator.Create();
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }

                private async Task<string> GenerateAndSaveRefreshToken(User user)
                {
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    return refreshToken;
                }

                public async Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request)
                {
                    var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
                    if (user is null)
                    {
                        return null;
                    }
                    return await CreateTokenResponse(user);
                } */
    }
}
