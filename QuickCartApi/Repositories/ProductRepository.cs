using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(int? categoryId, string? search, int page, int pageSize)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<Product?> GetByIdAsync(int id)
            => await context.Products
                   .Include(p => p.Category)
                   .Include(p => p.Images)
                   .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product> AddAsync(Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> CategoryExistsAsync(int categoryId) =>
            await context.Categories.AnyAsync(c => c.Id == categoryId);
    }
}
