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
        [Range(0, int.MaxValue)]
        // TODO: MAKE A CORRECT ERROR MESSAGE
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // TODO: ADD A VALIDATION 1 IMAGE MUST BE UPLOAD FOR CREATE PROD
        // [MinImages()]
        public List<IFormFile>? Images { get; set; }
    }
}
