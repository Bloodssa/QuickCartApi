using QuickCartApi.DTOs.Category;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var category = await categoryRepository.AddAsync(new Category { 
                Name = request.Name
            });

            return ToResponse(category);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category is null)
                return (false, "Category not found.");

            if (await categoryRepository.HasProductAsync(id))
                return (false, "Cannot delete a category that still has products");

            await categoryRepository.DeleteAsync(category);
            return (true, null);
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            var categories = await categoryRepository.GetAllAsync();
            return categories.Select(ToResponse).ToList();
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            return category is null ? null : ToResponse(category);
        }

        public async Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category is null)
                return null;

            category.Name = request.Name;
            await categoryRepository.UpdateAsync(category);

            return ToResponse(category);
        }

        private CategoryResponse ToResponse(Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            ProductCount = category.Products?.Count ?? 0
        };
    }
}
