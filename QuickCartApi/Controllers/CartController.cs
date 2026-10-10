using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Cart;
using QuickCartApi.Services.Interfaces;
using System.Security.Claims;

namespace QuickCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost]
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

        [HttpPatch("{id:int}")]
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

        [HttpDelete("{id:int}")]
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

        [HttpDelete]
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
