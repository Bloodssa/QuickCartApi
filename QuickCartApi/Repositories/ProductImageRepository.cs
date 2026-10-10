using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class ProductImageRepository(AppDbContext context) : BaseRepository<ProductImage>(context), IProductImageRepository
    {
        public async Task ClearDefaultAsync(int productId)
        {
            var images = await context.ProductImages
                .Where(i => i.ProductId == productId && i.IsDefault)
                .ToListAsync();

            foreach (var image in images)
                image.IsDefault = false;

            await context.SaveChangesAsync();
        }

        public override async Task<ProductImage?> GetByIdAsync(int id) =>
            await context.ProductImages.FirstOrDefaultAsync(i => i.Id == id);

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId) =>
            await context.ProductImages.Where(i => i.ProductId == productId).ToListAsync();
    }
}
