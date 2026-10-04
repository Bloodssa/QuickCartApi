namespace QuickCartApi.DTOs.Product
{
    public class ProductImageResponse
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
