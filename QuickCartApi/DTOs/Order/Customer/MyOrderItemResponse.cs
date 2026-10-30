namespace QuickCartApi.DTOs.Order.Customer
{
    public class MyOrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
