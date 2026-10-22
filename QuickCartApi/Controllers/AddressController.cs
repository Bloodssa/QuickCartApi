using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Address;
using QuickCartApi.Services.Interfaces;
using System.Security.Claims;
using QuickCartApi.Extensions;

namespace QuickCartApi.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
    public class AddressController(IAddressService addressService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<AddressResponse>>> GetAll()
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var address = await addressService.GetAllAsync(userId.Value);
            return Ok(address);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AddressResponse>> GetById([FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var address = await addressService.GetByIdAsync(userId.Value, id);

            if (address is null)
                return NotFound(new { message = "Address not found." });

            return Ok(address);
        }

        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        [HttpPost]
        public async Task<ActionResult<AddressResponse>> Create([FromBody] AddressRequest request)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var result = await addressService.CreateAsync(userId.Value, request);

            if (!result.Success)
                return result.ToProblem();

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id}, result.Value);
        }

        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AddressResponse>> Update([FromRoute] int id, [FromBody] AddressRequest request)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var result = await addressService.UpdateAsync(userId.Value, id, request);

            if (!result.Success)
                return result.ToProblem();

            return Ok(result.Value);
        }

        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        [HttpPatch("{id:int}/default")]
        public async Task<IActionResult> SetDefault([FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var result = await addressService.SetDefaultAsync(userId.Value, id);

            if (!result.Success)
                return result.ToProblem();

            return NoContent();
        }

        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized(new { message = "Unauthorized. Please log in first." });

            var result = await addressService.DeleteAsync(userId.Value, id);

            if (!result.Success)
                return result.ToProblem();

            return NoContent();
        }

        private int? GetUserId()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaim is null || !int.TryParse(userClaim.Value, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
