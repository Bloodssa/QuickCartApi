namespace QuickCartApi.Entities
{
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public Product Product { get; set; } = null!;
    }
}
