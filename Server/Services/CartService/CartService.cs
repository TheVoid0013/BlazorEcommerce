using BlazorEcommerce.Server.Services.AuthService;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;
        private readonly IFusionCache _fusionCache;
        private const string CACHE_KEY_PREFIX = "cart_";

        public CartService(DataContext context, IAuthService authService, IFusionCache fusionCache)
        {
            _context = context;
            _authService = authService;
            _fusionCache = fusionCache;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>()
            };

            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant == null)
                {
                    continue;
                }
                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity
                };

                result.Data.Add(cartProduct);
            }
            return result;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
        {
            cartItems.ForEach(cartItem => cartItem.UserId = _authService.GetUserId());
            _context.CartItems.AddRange(cartItems);
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            await InvalidateUserCartCache(_authService.GetUserId());

            return await GetDbCartProducts();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var userId = _authService.GetUserId();
            var cacheKey = GetCartCountCacheKey(userId);

            // Try to get from cache first
            var cachedCount = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return (await _context.CartItems
                        .Where(ci => ci.UserId == userId)
                        .ToListAsync()).Count;
                },
                TimeSpan.FromMinutes(5) // Cache for 5 minutes
            );

            return new ServiceResponse<int> { Data = cachedCount };
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
        {
            var userId = _authService.GetUserId();
            var cacheKey = GetCartProductsCacheKey(userId);

            // Try to get from cache first
            var cartItems = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.CartItems
                        .Where(ci => ci.UserId == userId)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(5)
            );

            return await GetCartProducts(cartItems);
        }

        public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
        {
            cartItem.UserId = _authService.GetUserId();

            var sameItem = await _context.CartItems.FirstOrDefaultAsync(ci => 
                ci.ProductId == cartItem.ProductId && 
                ci.ProductTypeId == cartItem.ProductTypeId && 
                ci.UserId == cartItem.UserId);

            if (sameItem == null)
            {
                _context.CartItems.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            await InvalidateUserCartCache(_authService.GetUserId());

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
        {
            var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => 
                ci.ProductId == cartItem.ProductId && 
                ci.ProductTypeId == cartItem.ProductTypeId && 
                ci.UserId == _authService.GetUserId());
                
            if (dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
            }

            dbCartItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            await InvalidateUserCartCache(_authService.GetUserId());

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => 
                ci.ProductId == productId && 
                ci.ProductTypeId == productTypeId && 
                ci.UserId == _authService.GetUserId());
                
            if (dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
            }

            _context.CartItems.Remove(dbCartItem);
            await _context.SaveChangesAsync();

            // Invalidate cache for this user
            await InvalidateUserCartCache(_authService.GetUserId());

            return new ServiceResponse<bool> { Data = true };
        }

        // Helper methods for cache management
        private string GetCartProductsCacheKey(int userId) => $"{CACHE_KEY_PREFIX}products_{userId}";
        private string GetCartCountCacheKey(int userId) => $"{CACHE_KEY_PREFIX}count_{userId}";

        private async Task InvalidateUserCartCache(int userId)
        {
            await _fusionCache.RemoveAsync(GetCartProductsCacheKey(userId));
            await _fusionCache.RemoveAsync(GetCartCountCacheKey(userId));
        }
    }
}