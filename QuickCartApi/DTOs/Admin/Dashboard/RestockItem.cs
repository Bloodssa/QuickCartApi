namespace QuickCartApi.DTOs.Admin.Dashboard
{
    public class RestockItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}
