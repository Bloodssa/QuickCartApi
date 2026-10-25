namespace QuickCartApi.DTOs.Order
{
    public class OrderListResponse
    {
        public List<OrderSummaryResponse> Orders { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
    }
}
