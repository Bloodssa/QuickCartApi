namespace QuickCartApi.DTOs.Order
{
    public class PaymentResponse
    {
        public string TransactionReference { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
