using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface ICartItemRepository : IBaseRepository<CartItem>
    {
        Task<List<CartItem>> GetByUserId(int userId);
        Task<CartItem?> GetByIdForUserAsync(int id, int userId);
        Task<CartItem?> GetByUserAndProductAsync(int userId, int productId);
        Task<int> DeleteByUserIdAsync(int userId);
    }
}
