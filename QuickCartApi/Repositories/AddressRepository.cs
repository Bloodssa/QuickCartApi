using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class AddressRepository(AppDbContext context) : BaseRepository<Address>(context), IAddressRepository
    {
        public async Task<List<Address>> GetByUserIdAsync(int userId)
            => await context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();

        public async Task<Address?> GetByIdForUserAsync(int id, int userId)
            => await context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        public async Task<int> CountByUserAsync(int userId)
            => await context.Addresses.CountAsync(a => a.UserId == userId);

        public async Task ClearDefaultAsync(int userId)
            => await context.Addresses
                .Where(a => a.UserId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));

        public async Task<bool> HasOrderAsync(int addressId)
            => await context.Orders.AnyAsync(o => o.AddressId == addressId);
    }
}
