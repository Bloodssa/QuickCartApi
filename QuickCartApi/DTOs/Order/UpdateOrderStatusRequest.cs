using QuickCartApi.Enums;

namespace QuickCartApi.DTOs.Order
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
