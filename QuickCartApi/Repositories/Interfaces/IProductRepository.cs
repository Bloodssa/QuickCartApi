using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(int? categoryId, string? search, int page, int pageSize);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}
