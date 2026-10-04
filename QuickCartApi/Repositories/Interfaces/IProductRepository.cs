using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(int? categoryId, string? search, int page, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}
