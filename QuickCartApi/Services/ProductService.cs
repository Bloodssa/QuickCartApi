using QuickCartApi.DTOs.Product;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class ProductService(
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            IFileStorageService storage,
            IHttpContextAccessor httpContextAccessor
        ) : IProductService
    {

        public async Task<ProductListResponse> GetAllAsync(int? categoryId, string? search, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            var (items, totalCount) = await productRepository.GetAllAsync(categoryId, search, page, pageSize);

            return new ProductListResponse
            {
                Products = items.Select(ToResponse).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalResults = totalCount
            };
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            return product is null ? null : ToResponse(product);
        }

        public async Task<(ProductResponse? Product, string? error)> CreateAsync(CreateProductRequest request)
        {
            if (!await productRepository.CategoryExistsAsync(request.CategoryId))
                return (null, "Category does not exists.");

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await productRepository.AddAsync(product);

            if (request.Images is { Count: > 0})
            {
                for (var i = 0; i < request.Images.Count; i++)
                {
                    var (success, fileName, _) = await storage.SaveImageAsync(request.Images[i]);
                    if (!success) continue;

                    await productImageRepository.AddAsync(new ProductImage
                    {
                        ProductId = created.Id,
                        ImageUrl = $"/uploads/products/{fileName}",
                        IsDefault = i == 0 // first image is the default image
                    });
                }
            }

            var withImages = await productRepository.GetByIdAsync(created.Id);
            return (ToResponse(withImages!), null);
        }

        public async Task<(bool Success, string? Error, ProductResponse? response)> UpdateAsync(int id, UpdateProductRequest request)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product is null) return (false, "Product not found.", null);

            if (!await productRepository.CategoryExistsAsync(request.CategoryId))
                return (false, "Category does not exists.", null)!;

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;

            var updated = await productRepository.UpdateAsync(product);
            return (true, null, ToResponse(updated));
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product is null) return (false, "Product not found.");

            if (product.Images is { Count: > 0 })
            {
                foreach (var image in product.Images)
                {
                    var fileName = Path.GetFileName(image.ImageUrl);
                    storage.DeleteImage(fileName);
                }
            }

            await productRepository.DeleteAsync(product);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteImageAsync(int productId, int imageId)
        {
            var image = await productImageRepository.GetByIdAsync(imageId);
            if (image is null || image.ProductId != productId)
                return (false, "Image not found.");

            var fileName = Path.GetFileName(image.ImageUrl);
            storage.DeleteImage(fileName);

            await productImageRepository.DeleteAsync(image);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> SetDefaultImageAsync(int productId, int imageId)
        {
            var image = await productImageRepository.GetByIdAsync(imageId);
            if (image is null || image.ProductId != productId)
                return (false, "Image not found.");

            await productImageRepository.ClearDefaultAsync(productId);
            image.IsDefault = true;
            await productImageRepository.UpdateAsync(image);

            return (true, null);
        }

        private ProductResponse ToResponse(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            CreatedAt = product.CreatedAt,
            Images = product.Images?.Select(i => new ProductImageResponse
            {
                Id = i.Id,
                ImageUrl = GetFullImageUrl(i.ImageUrl),
                IsDefault = i.IsDefault
            }).ToList() ?? []
        };

        private string GetFullImageUrl(string url)
        {
            var request = httpContextAccessor.HttpContext?.Request;

            if (request is null)
                return url;

            return $"{request.Scheme}://{request.Host}{url}";
        }
    }
}
