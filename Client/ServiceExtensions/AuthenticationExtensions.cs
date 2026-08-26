using BlazorEcommerce.Client.Services.AuthService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorEcommerce.Client
{
    public static class AuthenticationExtensions
    {
        public static WebAssemblyHostBuilder ConfigureAuthentication(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            return builder;
        }
    }
}