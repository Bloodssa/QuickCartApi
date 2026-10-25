using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Order;
using QuickCartApi.Extensions;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/admin/orders")]
    [ApiController]
    [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
    public class AdminOrdersController(IOrderService orderService) : ControllerBase
    {
        // GET api/v1/admin/orders?status=Pending&page=1: every customer's orders
        [HttpGet]
        public async Task<ActionResult<OrderListResponse>> Get([FromQuery] OrderQuery query)
            => Ok(await orderService.GetAllAsync(query));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderResponse>> GetById(int id)
        {
            var result = await orderService.GetByIdAsync(id);
            return result.Success ? Ok(result.Value) : result.ToProblem();
        }

        // PUT api/v1/admin/orders/42/status  { "status": "Confirmed", "paymentStatus": "Paid" }
        [HttpPut("{id:int}/status")]
        [EnableRateLimiting(RateLimitingExtension.WritePolicy)]
        public async Task<ActionResult<OrderResponse>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await orderService.UpdateStatusAsync(id, request);
            return result.Success ? Ok(result.Value) : result.ToProblem();
        }
    }
}
