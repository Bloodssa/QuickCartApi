using QuickCartApi.Common;
using QuickCartApi.DTOs.Cart;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class CartService(
            ICartItemRepository cartRepository,
            IProductRepository productRepository,
            IHttpContextAccessor httpContextAccessor
        ) : ICartService
    {
        public async Task<ServiceResult<CartResponse>> CreateAsync(int userId, AddToCartRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);
            if (product is null)
                return Result.Fail<CartResponse>(ErrorKind.NotFound, "Product not found.");

            if (product.StockQuantity == 0) // ERR-QC-102
                return Result.Fail<CartResponse>(ErrorKind.Validation, "This product is currently out of stock.");

            var existing = await cartRepository.GetByUserAndProductAsync(userId, request.ProductId);
            var newQuantity = (existing?.Quantity ?? 0) + request.Quantity;

            if (newQuantity > product.StockQuantity) // ERR-QC-103 / BR-QC-02
                return Result.Fail<CartResponse>(ErrorKind.Validation, "Requested quantity exceeds available stock.");

            if (existing is null)
            {
                await cartRepository.AddAsync(new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = newQuantity
                });
            } else
            {
                existing.Quantity = newQuantity;
                await cartRepository.UpdateAsync(existing);
            }

            return Result.Ok(await GetCartAsync(userId));
        }

        public async Task<CartResponse> GetCartAsync(int userId) => 
            ToResponse(await cartRepository.GetByUserId(userId));

        public async Task<ServiceResult<CartResponse>> UpdateAsync(int userId, int cartItemId, UpdateCartItemRequest request)
        {
            var item = await cartRepository.GetByIdForUserAsync(cartItemId, userId);
            if (item is null)
                return Result.Fail<CartResponse>(ErrorKind.NotFound, "Cart item not found.");

            if (request.Quantity > item.Product.StockQuantity)
                return Result.Fail<CartResponse>(ErrorKind.Validation, "Requested quantity exceeds available stock.");

            item.Quantity = request.Quantity;
            await cartRepository.UpdateAsync(item);

            return Result.Ok(await GetCartAsync(userId));
        }

        public async Task<ServiceResult<CartResponse>> DeleteAsync(int userId, int cartItemId)
        {
            var item = await cartRepository.GetByIdForUserAsync(cartItemId, userId);
            if (item is null)
                return Result.Fail<CartResponse>(ErrorKind.Validation, "Cart item not found.");

            await cartRepository.DeleteAsync(item);
            return Result.Ok(await GetCartAsync(userId));
        }

        public async Task<ServiceResult> ClearAsync(int userId)
        {
            var items = await cartRepository.GetByUserId(userId);

            if (!items.Any())
                return Result.Fail(ErrorKind.Validation, "Your cart is already empty.");

            await cartRepository.DeleteByUserIdAsync(userId);

            return Result.Ok();
        }

        private CartResponse ToResponse(IEnumerable<CartItem> items)
        {
            var mapped = items.Select(ci => new CartItemResponse
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ImageUrl = DefaultImageUrl(ci.Product.Images) is string url ? GetFullImageUrl(url) : null,
                UnitPrice = ci.Product.Price,
                Quantity = ci.Quantity,
                SubTotal = ci.Product.Price * ci.Quantity,
                StockQuantity = ci.Product.StockQuantity
            }).ToList();

            return new CartResponse
            {
                Items = mapped,
                TotalItems = mapped.Sum(m => m.Quantity),
                SubTotal = mapped.Sum(m => m.SubTotal)
            };
        }

        // get the default image of the product, if no default get the first image
        private static string? DefaultImageUrl(IEnumerable<ProductImage> images) =>
            images.FirstOrDefault(i => i.IsDefault)?.ImageUrl ?? images.FirstOrDefault()?.ImageUrl;

        private string GetFullImageUrl(string url)
        {
            var request = httpContextAccessor.HttpContext?.Request;

            if (request is null)
                return url;

            return $"{request.Scheme}://{request.Host}{url}";
        }
    }
}
