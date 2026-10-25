using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.DTOs.Order;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuickCartApi.Repositories
{
    public class OrderRepository(AppDbContext context) : BaseRepository<Order>(context), IOrderRepository
    {
        public Task<(List<Order> Items, int totalCount)> GetAllAsync(OrderQuery request) =>
            PageAsync(context.Orders, request);

        public async Task<Order?> GetByIdWithDetailsAsync(int id) =>
            await context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Transaction)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == id);

        public Task<(List<Order> Items, int totalCount)> GetByUserIdAsync(int userId, OrderQuery query) =>
            PageAsync(context.Orders.Where(o => o.UserId == userId), query);

        public async Task<bool> OrderNumberExistsAsync(string orderNumber) =>
            await context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);

        // shared by both lists: filter -> count -> sort newest first -> page
        private static async Task<(List<Order> Items, int TotalCount)> PageAsync(IQueryable<Order> source, OrderQuery q)
        {
            if (q.Status.HasValue)
                source = source.Where(o => o.Status == q.Status.Value);

            var total = await source.CountAsync();   // counted BEFORE Skip/Take

            var items = await source
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Transaction)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .AsSplitQuery()
                .ToListAsync();

            return (items, total);
        }
    }
}
