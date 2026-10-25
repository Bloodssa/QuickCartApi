using QuickCartApi.Enums;

namespace QuickCartApi.DTOs.Order
{
    public class OrderQuery
    {
        public OrderStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
