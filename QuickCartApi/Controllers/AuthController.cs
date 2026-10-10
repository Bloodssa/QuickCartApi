using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCartApi.DTOs.Auth;
using QuickCartApi.Entities;
using QuickCartApi.Services.Interfaces;
using System.Security.Claims;

namespace QuickCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            var user = await authService.RegisterAsync(request);

            if (user is null)
                return Conflict(new { message = "Username already exists." });

            return CreatedAtAction(nameof(Register), user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await authService.LoginAsync(request, Response);

            if (result is null)
                return Unauthorized(new { message = "Invalid Credentials." });

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refrfesh()
        {
            var result = await authService.RefreshAsync(Request, Response);

            if (result is null)
                return Unauthorized(new { message = "Invalid or expired token, Please try to login again" });

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync(Request, Response);

            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> AuthenticatedOnlyEndPoint()
        {
            // get the user id from the json playload
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await authService.GetCurrentUserAsync(userId);

            if (user is null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok(new { message = "You are a admin" });
        }
    }
}
