using QuickCartApi.Common;
using QuickCartApi.DTOs.Order;

namespace QuickCartApi.Services.Interfaces
{
    public interface IOrderService
    {
        // customer service
        Task<ServiceResult<OrderResponse>> PlaceOrderAsync(int userId, PlaceOrderRequest request);
        Task<OrderListResponse> GetMyOrdersAsync(int userId, OrderQuery query);
        Task<ServiceResult<OrderResponse>> GetMyOrderAsync(int userId, int orderId);
        Task<ServiceResult<OrderResponse>> CancelMyOrderAsync(int userId, int orderId);

        // admin service
        Task<OrderListResponse> GetAllAsync(OrderQuery query);
        Task<ServiceResult<OrderResponse>> GetByIdAsync(int orderId);
        Task<ServiceResult<OrderResponse>> UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request);
    }
}
