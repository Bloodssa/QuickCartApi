namespace QuickCartApi.DTOs.Order
{
    public class OrderSummaryResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
