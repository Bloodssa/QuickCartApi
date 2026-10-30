namespace QuickCartApi.DTOs.Product
{
    public class StockSummaryResponse
    {
        public int TotalUnits { get; set; }
        public int LowCount { get; set; }
        public int OutCount { get; set; }
        public int LowStockThreshold { get; set; }
    }
}
