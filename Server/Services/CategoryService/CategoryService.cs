using ZiggyCreatures.Caching.Fusion;

namespace BlazorEcommerce.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        private readonly IFusionCache _fusionCache;

        public CategoryService(DataContext context, IFusionCache fusionCache)
        {
            _context = context;
            _fusionCache = fusionCache;
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var cacheKey = "categories_all";

            var categories = await _fusionCache.GetOrSetAsync(
                cacheKey,
                async cancellationToken =>
                {
                    return await _context.Categories.ToListAsync();
                },
                TimeSpan.FromHours(2) 
            );

            return new ServiceResponse<List<Category>>
            {
                Data = categories
            };
        }
    }
}