using QuickCartApi.Enums;

namespace QuickCartApi.Entities
{
    public class Transaction : BaseEntity
    {
        public int OrderId { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime TransactionDate { get; set; }
        public Order Order { get; set; } = null!;
    }
}
