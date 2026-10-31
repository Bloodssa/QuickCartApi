namespace QuickCartApi.DTOs.Admin.Reports
{
    public class SalesReportSummary
    {
        public decimal PaidRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int CancelledOrders { get; set; }
    }
}
