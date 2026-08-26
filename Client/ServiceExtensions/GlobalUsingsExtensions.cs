using Microsoft.AspNetCore.Components.WebAssembly.Hosting;  // ← Add this line

namespace BlazorEcommerce.Client
{
    public static class GlobalUsingsExtensions
    {
        public static WebAssemblyHostBuilder ConfigureGlobalUsings(this WebAssemblyHostBuilder builder)
        {
            // Global usings are now configured at the top of each file
            // This method is kept for consistency
            return builder;
        }
    }
}