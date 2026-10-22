using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.DTOs.Product;
using QuickCartApi.Entities;
using QuickCartApi.Enums;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class ProductRepository(AppDbContext context) : BaseRepository<Product>(context), IProductRepository
    {
        private const int TrendingDays = 30;

        public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(ProductQuery q)
        {
            IQueryable<Product> query = context.Products;

            if (q.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == q.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(p => p.Name.Contains(q.Search));

            if (q.Featured.HasValue)
                query = query.Where(p => p.IsFeatured == q.Featured.Value);

            var totalCount = await query.CountAsync();

            var items = await ApplySort(query, q.Sort ?? ProductSort.Newest)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking()
                .AsSplitQuery()
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public override async Task<Product?> GetByIdAsync(int id)
            => await context.Products
                   .Include(p => p.Category)
                   .Include(p => p.Images)
                   .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> CategoryExistsAsync(int categoryId) =>
            await context.Categories.AnyAsync(c => c.Id == categoryId);

        private static IOrderedQueryable<Product> ApplySort(IQueryable<Product> query, ProductSort sort)
        {
            var since = DateTime.UtcNow.AddDays(-TrendingDays);

            return sort switch
            {
                ProductSort.PriceAsc => query.OrderBy(p => p.Price).ThenByDescending(p => p.Id),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.Price).ThenByDescending(p => p.Id),
                ProductSort.Popular => query
                    .OrderByDescending(p => p.OrderItems
                        .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
                        .Sum(oi => oi.Quantity)
                    ).ThenByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.Id),

                ProductSort.Trending => query
                    .OrderByDescending(p => p.OrderItems
                    .Where(oi => oi.Order.Status != OrderStatus.Cancelled && oi.Order.OrderDate >= since)
                    .Sum(oi => oi.Quantity))
                    .ThenByDescending(p => p.CreatedAt) 
                    .ThenByDescending(p => p.Id),

                _ => query                                   // Newest
                    .OrderByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.Id)
            };
        }
    }
}
