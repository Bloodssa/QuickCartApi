using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
        Task<List<Address>> GetByUserIdAsync(int userId);
        Task<Address?> GetByIdForUserAsync(int id, int userId);
        Task<int> CountByUserAsync(int userId);
        Task ClearDefaultAsync(int userId);
        Task<bool> HasOrderAsync(int addressId);
    }
}
