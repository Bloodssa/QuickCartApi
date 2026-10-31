namespace QuickCartApi.DTOs.Admin.Dashboard
{
    public class TopProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
