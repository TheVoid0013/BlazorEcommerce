using BlazorEcommerce.Server.Services.CategoryService;
using BlazorEcommerce.Server.Services.ProductService;
using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using BlazorEcommerce.Server.Services.OrderService;

namespace BlazorEcommerce.Server.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>()
                .AddScoped<ICategoryService, CategoryService>()
                .AddScoped<ICartService, CartService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}