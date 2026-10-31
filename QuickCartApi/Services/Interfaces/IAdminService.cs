using QuickCartApi.Common;
using QuickCartApi.DTOs.Admin.Dashboard;
using QuickCartApi.DTOs.Admin.Reports;

namespace QuickCartApi.Services.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardResponse> GetDashboardDataAsync();
        Task<ServiceResult<SalesReportResponse>> GetSalesReportAsync(DateOnly from, DateOnly to, int page, int pageSize);
        Task<ServiceResult<byte[]>> GetSalesReportPdfAsync(DateOnly from, DateOnly to);
    }
}
