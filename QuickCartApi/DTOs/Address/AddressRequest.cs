using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Address
{
    public class AddressRequest
    {
        [Required] 
        [MaxLength(255)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?:\+63|0)9\d{9}$", ErrorMessage = "Contact number must be a valid Philippine mobile number.")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Barangay { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Province { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postal code must be 4 digits.")]
        public string PostalCode { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}
