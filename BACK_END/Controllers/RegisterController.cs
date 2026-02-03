using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers.RegisterController
{
  [Route("api/[controller]")]
  [ApiController]
  public class RegisterController : ControllerBase
  {
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterController(UserManager<IdentityUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpPost("/register/{role}")]
    public async Task<IActionResult> RegisterService([FromBody] RegisterModel model, [FromRoute] string role)
    {
      try
      {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {

          Claim[] userClaims =
              [
                  new Claim(ClaimTypes.Email, model.Email),
                                new Claim(ClaimTypes.Role, role)
              ];

          await _userManager.AddClaimsAsync(user, userClaims);
          return Ok("Compte créé avec succès.");

        }

        else
        {
          throw new Exception("Erreur lors de la création du compte.");
        }

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}