using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class CategoryRepository(AppDbContext context) : BaseRepository<Category>(context), ICategoryRepository
    {
        // return a list of category with its product
        public async Task<List<Category>> GetAllAsync()
            => await context.Categories
                .Include(c => c.Products)
                .ToListAsync();

        public override async Task<Category?> GetByIdAsync(int id)
            => await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

        // check if the category has a product
        // @returns bool
        public async Task<bool> HasProductAsync(int categoryId)
            => await context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}
