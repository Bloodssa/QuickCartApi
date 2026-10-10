using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IProductImageRepository : IBaseRepository<ProductImage>
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task ClearDefaultAsync(int productId);
    }
}
