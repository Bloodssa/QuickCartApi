using QuickCartApi.DTOs.Order;
using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<(List<Order> Items, int totalCount)> GetByUserIdAsync(int userId, OrderQuery query);
        Task<(List<Order> Items, int totalCount)> GetAllAsync(OrderQuery request);
        Task<bool> OrderNumberExistsAsync(string orderNumber);
    }
}
