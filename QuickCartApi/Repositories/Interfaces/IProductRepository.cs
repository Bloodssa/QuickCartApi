using QuickCartApi.DTOs.Product;
using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<(List<Product> Items, int TotalCount)> GetAllAsync(ProductQuery pq);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}
