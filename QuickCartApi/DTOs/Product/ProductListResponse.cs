namespace QuickCartApi.DTOs.Product
{
    public class ProductListResponse
    {
        public List<ProductResponse> Products { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
    }
}
