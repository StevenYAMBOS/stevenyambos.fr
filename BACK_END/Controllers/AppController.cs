using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Route("user/[controller]")]
[ApiController]
public class ApplicationController : Controller
{

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminManagerPolicy")]
  [HttpGet("/admin")]
  public IActionResult Admin()
  {
    return Ok("admin only");
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
  [HttpGet("/user")]
  public IActionResult Users()
  {
    return Ok("users only");
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminManagerUserPolicy")]
  [HttpGet("/everyone")]
  public IActionResult EveryOne()
  {
    return Ok("hello everyone");
  }

}