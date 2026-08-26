using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorEcommerce.Client
{
    public static class HttpClientExtensions
    {
        public static WebAssemblyHostBuilder ConfigureHttpClients(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
            });

            return builder;
        }
    }
}