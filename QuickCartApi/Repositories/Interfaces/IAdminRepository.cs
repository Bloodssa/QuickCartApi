using QuickCartApi.Common;
using QuickCartApi.DTOs.Admin.Dashboard;
using QuickCartApi.DTOs.Admin.Reports;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<DashboardResponse> GetDashboardAsync();
        Task<SalesReportSummary> GetSummaryAsync(ReportRange range);
        Task<List<RevenueDay>> GetRevenueByDayAsync(ReportRange range);
        Task<List<TopProduct>> GetTopProductsAsync(ReportRange range, int take);
        Task<(List<SalesTransactionRow> Rows, int Total)> GetTransactionsAsync(ReportRange range, int skip, int take);
    }
}
