namespace QuickCartApi.DTOs.Admin.Reports
{
    public class SalesTransactionRow
    {
        public DateTime Date { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
