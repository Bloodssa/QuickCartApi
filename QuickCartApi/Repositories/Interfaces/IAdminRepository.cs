using QuickCartApi.DTOs.Admin.Dashboard;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<DashboardResponse> GetDashboardAsync();
    }
}
