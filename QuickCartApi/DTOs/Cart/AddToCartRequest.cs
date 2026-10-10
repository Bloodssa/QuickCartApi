using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Cart
{
    public class AddToCartRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
