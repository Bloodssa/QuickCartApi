using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class CartItemRepository(AppDbContext context) : BaseRepository<CartItem>(context), ICartItemRepository
    {
        public async Task<int> DeleteByUserIdAsync(int userId)
            => await context.CartItems.Where(ci => ci.UserId == userId).ExecuteDeleteAsync();

        public async Task<CartItem?> GetByIdForUserAsync(int id, int userId)
            => await context.CartItems
                    .Include(ci => ci.Product)
                    .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.Id == id);

        public async Task<CartItem?> GetByUserAndProductAsync(int userId, int productId)
            => await context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        public async Task<List<CartItem>> GetByUserId(int userId)
            => await context.CartItems
                .AsNoTracking()
                .Include(ci => ci.Product).ThenInclude(p => p.Images)
                .Where(ci => ci.UserId == userId)
                .OrderBy(ci => ci.Id)
                .ToListAsync();

        public async Task<List<CartItem>> GetByIdsForUserAsync(int userId, List<int> ids) =>
            await context.CartItems
                .AsNoTracking()
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId && ids.Contains(ci.Id))
                .ToListAsync();

        public async Task<int> DeleteByIdsAsync(int userId, List<int> ids) =>
            await context.CartItems
                .Where(ci => ci.UserId == userId && ids.Contains(ci.Id))
                .ExecuteDeleteAsync();
    }
}
