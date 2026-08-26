using ZiggyCreatures.Caching.Fusion;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IFusionCache _fusionCache;
        private const string CACHE_KEY_PREFIX = "product_";

        public ProductService(DataContext context, IFusionCache fusionCache)
        {
            _context = context;
            _fusionCache = fusionCache;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            var cacheKey = GetProductCacheKey(productId);

            // Try to get product from cache
            var product = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Include(p => p.Variants)
                        .ThenInclude(v => v.ProductType)
                        .FirstOrDefaultAsync(p => p.Id == productId);
                },
                TimeSpan.FromHours(1) // Cache for 1 hour
            );

            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist";
            }
            else
            {
                response.Data = product;
            }
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}all";

            var products = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Include(p => p.Variants)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(30) // Cache for 30 minutes
            );

            return new ServiceResponse<List<Product>>
            {
                Data = products
            };
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}category_{categoryUrl.ToLower()}";

            var products = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                        .Include(p => p.Variants)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(30)
            );

            return new ServiceResponse<List<Product>>
            {
                Data = products
            };
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}search_{searchText.ToLower()}";

            return await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || 
                                    p.Description.ToLower().Contains(searchText.ToLower()))
                        .Include(p => p.Variants)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(15) // Shorter cache for search results
            );
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchPeoducts(string searchText, int page)
        {
            var pageResult = 2f;
            
            // Get all matching products (cached)
            var allProducts = await FindProductsBySearchText(searchText);
            
            var pageCount = Math.Ceiling(allProducts.Count() / pageResult);
            
            // For pagination, we need to get the specific page from database
            // Cache the paginated result separately
            var cacheKey = $"{CACHE_KEY_PREFIX}search_{searchText.ToLower()}_page_{page}";

            var products = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || 
                                    p.Description.ToLower().Contains(searchText.ToLower()))
                        .Include(p => p.Variants)
                        .Skip((page - 1) * (int)pageResult)
                        .Take((int)pageResult)
                        .ToListAsync();
                },
                TimeSpan.FromMinutes(15)
            );

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };
            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductsSearchSuggestions(string searchText)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}suggestions_{searchText.ToLower()}";

            var result = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    var products = await FindProductsBySearchText(searchText);
                    List<string> suggestions = new List<string>();

                    foreach (var product in products)
                    {
                        if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            suggestions.Add(product.Title);
                        }

                        if (product.Description != null)
                        {
                            var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                            var words = product.Description.Split().Select(s => s.Trim(punctuation));

                            foreach (var word in words)
                            {
                                if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && 
                                    !suggestions.Contains(word))
                                {
                                    suggestions.Add(word);
                                }
                            }
                        }
                    }

                    return suggestions;
                },
                TimeSpan.FromMinutes(10) // Short cache for suggestions
            );

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}featured";

            var products = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Products
                        .Where(p => p.Featured)
                        .Include(p => p.Variants)
                        .ToListAsync();
                },
                TimeSpan.FromHours(2) // Cache featured products longer
            );

            return new ServiceResponse<List<Product>>
            {
                Data = products
            };
        }

        // Helper method for cache invalidation (call this when products are updated)
        public async Task InvalidateProductCache(int? productId = null)
        {
            if (productId.HasValue)
            {
                await _fusionCache.RemoveAsync(GetProductCacheKey(productId.Value));
            }
            
            // Invalidate all product caches
            await _fusionCache.RemoveAsync($"{CACHE_KEY_PREFIX}all");
            await _fusionCache.RemoveAsync($"{CACHE_KEY_PREFIX}featured");
            
            // You might want to invalidate category caches too
            // This would need a list of categories or pattern-based invalidation
        }

        private string GetProductCacheKey(int productId) => $"{CACHE_KEY_PREFIX}{productId}";
    }
}