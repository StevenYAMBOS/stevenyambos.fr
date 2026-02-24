namespace Portfolio.Services;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Portfolio.Models;

public class TokenService(ILogger<TokenService> logger, IConfiguration configuration)
{
  private const int ExpirationMinutes = 30;
  private readonly ILogger<TokenService> _logger = logger;

  public string CreateToken(ApplicationUser user)
  {
    var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
    var token = CreateJwtToken(
        CreateClaims(user),
        CreateSigningCredentials(),
        expiration
    );
    var tokenHandler = new JwtSecurityTokenHandler();

    _logger.LogInformation("JWT Token créé");

    return tokenHandler.WriteToken(token);
  }

  private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
      DateTime expiration) =>
      new(
        configuration.GetValue<string>("AppSettings:Issuer"),
        configuration.GetValue<string>("AppSettings:Audience"),
        claims,
        expires: expiration,
        signingCredentials: credentials
      );

  private List<Claim> CreateClaims(ApplicationUser user)
  {
    // var jwtSub = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"];

    try
    {
      var claims = new List<Claim>
            {
                // new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

      return claims;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  private SigningCredentials CreateSigningCredentials()
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

    // var symmetricSecurityKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];

    return new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256
    );
  }
}