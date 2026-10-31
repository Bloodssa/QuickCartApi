using QuickCartApi.DTOs.Admin.Dashboard;

namespace QuickCartApi.DTOs.Admin.Reports
{
    public class SalesReportResponse
    {
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public SalesReportSummary Summary { get; set; } = new();
        public List<RevenueDay> RevenueByDay { get; set; } = [];     // reused from the dashboard DTOs
        public List<TopProduct> TopProducts { get; set; } = [];      // reused from the dashboard DTOs
        public List<SalesTransactionRow> Transactions { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
    }
}
