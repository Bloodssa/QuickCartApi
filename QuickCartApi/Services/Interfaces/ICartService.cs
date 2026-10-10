using QuickCartApi.Common;
using QuickCartApi.DTOs.Cart;

namespace QuickCartApi.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(int userId);
        Task<ServiceResult<CartResponse>> CreateAsync(int userId, AddToCartRequest request);
        Task<ServiceResult<CartResponse>> UpdateAsync(int userId, int cartItemId, UpdateCartItemRequest request);
        Task<ServiceResult<CartResponse>> DeleteAsync(int userId, int cartItemId);
        Task<ServiceResult> ClearAsync(int userId);
    }
}
