global using BlazorEcommerce.Data.Data;
global using BlazorEcommerce.Server.Services.CategoryService;
global using BlazorEcommerce.Server.Services.ProductService;
global using BlazorEcommerce.Shared;
global using Microsoft.EntityFrameworkCore;

using BlazorEcommerce.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure services using extension methods
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddBusinessServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFusionCache(); 
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure pipeline
app.ConfigurePipeline();

app.Run();