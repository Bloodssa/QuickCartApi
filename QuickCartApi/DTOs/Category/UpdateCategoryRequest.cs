using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }
}
