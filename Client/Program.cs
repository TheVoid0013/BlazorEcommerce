using BlazorEcommerce.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure services using extension methods
builder.ConfigureGlobalUsings();
builder.ConfigureServices();
builder.ConfigureAuthentication();
builder.ConfigureHttpClients();

await builder.Build().RunAsync();