using Blazored.LocalStorage;
using BlazorEcommerce.Client.Services.AuthService;
using BlazorEcommerce.Client.Services.CartService;
using BlazorEcommerce.Client.Services.CategoryService;
using BlazorEcommerce.Client.Services.OrderService;
using BlazorEcommerce.Client.Services.ProductService;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorEcommerce.Client
{
    public static class ServiceCollectionExtensions
    {
        public static WebAssemblyHostBuilder ConfigureServices(this WebAssemblyHostBuilder builder)
        {
            // Add Blazored LocalStorage
            builder.Services.AddBlazoredLocalStorage();

            // Register all services
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            return builder;
        }
    }
}