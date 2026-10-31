using QuickCartApi.DTOs.Admin.Dashboard;

namespace QuickCartApi.Services.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardResponse> GetDashboardDataAsync();
    }
}
