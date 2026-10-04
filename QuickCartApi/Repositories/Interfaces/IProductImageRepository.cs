using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<ProductImage?> GetByIdAsync(int id);
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task<ProductImage> AddAsync(ProductImage image);
        Task UpdateAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
        Task ClearDefaultAsync(int productId);
    }
}
