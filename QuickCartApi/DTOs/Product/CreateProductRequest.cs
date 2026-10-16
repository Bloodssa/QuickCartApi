using QuickCartApi.Validators;
using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Product
{
    public class CreateProductRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [MinImages(1, ErrorMessage = "Upload at least one product image.")]
        public List<IFormFile>? Images { get; set; }
    }
}
