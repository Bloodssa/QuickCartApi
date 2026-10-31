using QuickCartApi.DTOs.Admin.Dashboard;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class AdminService(
        IAdminRepository adminRepository
    ) : IAdminService
    {
        public async Task<DashboardResponse> GetDashboardDataAsync()
        {
            return await adminRepository.GetDashboardAsync();
        }
    }
}
