using QuickCartApi.Common;
using QuickCartApi.DTOs.Address;

namespace QuickCartApi.Services.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressResponse>> GetAllAsync(int userId);
        Task<AddressResponse?> GetByIdAsync(int userId, int addressId);
        Task<ServiceResult<AddressResponse>> CreateAsync(int userId, AddressRequest request);
        Task<ServiceResult<AddressResponse>> UpdateAsync(int userId, int id, AddressRequest request);
        Task<ServiceResult> DeleteAsync(int userId, int id);
        Task<ServiceResult> SetDefaultAsync(int userId, int id);
    }
}
