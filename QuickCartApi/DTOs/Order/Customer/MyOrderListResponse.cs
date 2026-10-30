public class MyOrderListResponse
{
    public List<MyOrderSummaryResponse> Orders { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalResults { get; set; }
}