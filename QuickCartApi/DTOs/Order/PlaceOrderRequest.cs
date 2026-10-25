using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Order
{
    public class PlaceOrderRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a delivery address")]
        public int AddressId { get; set; }

        [Required]
        [AllowedValues("COD", "Wallet", "Card")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Select at least one item to check out")]
        public List<int> CartItemIds { get; set; } = [];
    }
}
