using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuickCartApi.DTOs.Admin.Dashboard;
using QuickCartApi.Extensions;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Controllers
{
    [ApiVersion(1)]
    [EnableRateLimiting(RateLimitingExtension.ReadPolicy)]
    [Route("api/v{version:apiVersion}/admin")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponse>> Dashboard()
            => await adminService.GetDashboardDataAsync();
    }
}
