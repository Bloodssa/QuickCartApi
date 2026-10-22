using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Cart;
using QuickCartApi.Extensions;
using QuickCartApi.Services.Interfaces;
using System.Security.Claims;

namespace QuickCartApi.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting(RateLimitingExtension.ReadPolicy)] // deafault ratelimit for read
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized( new { message = "Unauthorized, Please log in first." });

            return Ok(await cartService.GetCartAsync(userId.Value)); 
        }

        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        [HttpPost("items")]
        public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(new { message = "Unauthorized, Please log in first." });

            var result = await cartService.CreateAsync(userId.Value, request);

            if (!result.Success)
                return result.ToProblem();

            return Ok(result.Value);
        }

        [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
        [HttpPatch("items/{id:int}")]
        public async Task<ActionResult<CartResponse>> UpdateCartItemQuantity([FromBody] UpdateCartItemRequest request, [FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized, Please log in first." });

            var result = await cartService.UpdateAsync(userId.Value, id, request);

            if (!result.Success)
                return result.ToProblem();

            return Ok(result.Value);
        }

        [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
        [HttpDelete("items/{id:int}")]
        public async Task<ActionResult> RemoveCartItem([FromRoute] int id)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(new { message = "Unauthorized, Please log in first." });

            var result = await cartService.DeleteAsync(userId.Value, id);
            if (!result.Success)
                return result.ToProblem();

            return NoContent();
        }

        [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
        [HttpDelete("items")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized, Please log in first." });

            var result = await cartService.ClearAsync(userId.Value);

            if (!result.Success)
                return result.ToProblem();

            return NoContent();
        }

        private int? GetUserId()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim is null || !int.TryParse(userClaim.Value, out int userId))
                return null;

            return userId;
        }
    }
}
