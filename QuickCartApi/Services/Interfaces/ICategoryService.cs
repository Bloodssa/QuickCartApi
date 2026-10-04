using QuickCartApi.DTOs.Category;

namespace QuickCartApi.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
        Task<CategoryResponse?> UpdateAsync(int id, UpdateCategoryRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
