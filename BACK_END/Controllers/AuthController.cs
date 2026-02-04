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
            Log.Information("Register endpoint called for user: {Username}", request.Username, DateTime.Now);
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                Log.Error("Registration failed for user: {Username}", request.Username, DateTime.Now);
                return BadRequest("User already exists.");
            }
            Log.Information("User registered successfully: {Username}", request.Username, DateTime.Now);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            Log.Information("Login endpoint called for user: {Username}", request.Username, DateTime.Now);
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                Log.Error("Login failed for user: {Username}", request.Username, DateTime.Now);
                return BadRequest("Invalid username or password.");
            }
            Log.Information("User logged in successfully: {Username}", request.Username, DateTime.Now);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            Log.Information("Refresh token endpoint called for user ID: {UserId}", request.UserId, DateTime.Now);
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                Log.Error("Refresh token failed for user ID: {UserId}", request.UserId, DateTime.Now);
                return Unauthorized("Invalid refresh token.");
            }
            Log.Information("Tokens refreshed successfully for user ID: {UserId}", request.UserId, DateTime.Now);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            Log.Information("Authenticated endpoint accessed by user: {Username}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            Log.Information("Admin endpoint accessed by user: {Username}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }

    }
}
