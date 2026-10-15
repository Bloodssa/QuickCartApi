using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Auth;
using QuickCartApi.DTOs.Users;
using QuickCartApi.Services.Interfaces;
using System.Security.Claims;

namespace QuickCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await authService.RegisterAsync(request);

            if (!result.Success)
                return result.ToProblem();

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await authService.LoginAsync(request, Response);

            if (result is null)
                return Unauthorized(new { message = "Invalid username or password" });

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
