using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Product
{
    public class AdjustStockRequest
    {
        [Range(-100000, 100000, ErrorMessage = "Change must be between -100000 and 100000.")]
        public int Delta { get; set; }
    }
}
