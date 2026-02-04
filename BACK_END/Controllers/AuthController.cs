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
            Console.WriteLine("Register endpoint called for user: {Username}", request.Username, DateTime.Now);
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                Console.WriteLine("Registration failed for user: {Username}", request.Username, DateTime.Now);
                return BadRequest("User already exists.");
            }
            Console.WriteLine("User registered successfully: {Username}", request.Username, DateTime.Now);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            Console.WriteLine("Login endpoint called for user: {Username}", request.Username, DateTime.Now);
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                Console.WriteLine("Login failed for user: {Username}", request.Username, DateTime.Now);
                return BadRequest("Invalid username or password.");
            }
            Console.WriteLine("User logged in successfully: {Username}", request.Username, DateTime.Now);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            Console.WriteLine("Refresh token endpoint called for user ID: {UserId}", request.UserId, DateTime.Now);
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                Console.WriteLine("Refresh token failed for user ID: {UserId}", request.UserId, DateTime.Now);
                return Unauthorized("Invalid refresh token.");
            }
            Console.WriteLine("Tokens refreshed successfully for user ID: {UserId}", request.UserId, DateTime.Now);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            Console.WriteLine("Authenticated endpoint accessed by user: {Username}", User.Identity?.Name, DateTime.Now);
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            Console.WriteLine("Admin endpoint accessed by user: {Username}", User.Identity?.Name, DateTime.Now);
            return Ok("You are Admin!");
        }

    }
}
