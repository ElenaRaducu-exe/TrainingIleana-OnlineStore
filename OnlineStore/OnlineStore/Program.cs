using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

using OnlineStore.Components;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Models.Configuration;
using OnlineStore.Services.Backend;
using OnlineStore.Services.Backend.Contracts;
using OnlineStore.Services.Frontend;
using OnlineStore.Services.Frontend.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Razor components
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//HttpClient
builder.Services.AddHttpClient(); 

// Database
builder.Services.AddDbContext<OnlineStoreContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Database configuration
builder.Services.AddScoped((x) => new DbConfiguration
{
    OnlineStoreDb = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty
});

// MudBlazor
builder.Services.AddMudServices();

// JWT Authentication
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = JwtTokenValidationOptionsFactory.CreateTokenValidationParameters(builder.Configuration);
        });
builder.Services.AddAuthorization(); 
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
            provider.GetRequiredService<JWTAuthenticationStateProvider>());

//builder.Services.AddScoped<JWTAuthorizationHandler>();
//builder.Services.AddHttpClient("AuthenticatedUser").AddHttpMessageHandler<JWTAuthorizationHandler>();
//builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddHttpClient<IApiClient, ApiClient>("AuthenticatedUser");

//Controllers
//builder.Services.AddControllers();  
builder.Services.AddControllers(options =>
{
    // Forces all string return values to be JSON-serialized
    // instead of falling back to the plain text formatter.

    // the line removes the deafult plain-text output formatter 
    // preventing the API from returning string responses as text/plain
    // useful when all responses should be JSON-formatted, even for string return types 
    // enforcing consistent types across the API
    options.OutputFormatters.RemoveType<StringOutputFormatter>();

    // the line is used to remove the built-in formatter that automatically returns HTTP 204 No content 
    // when an action returns null or empty result instead of 200 OK with an exmpty body 
    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
});

// Services - Backend
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandsService, BrandsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<ICartProductsService, CartProductsService>(); 
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrderService,  OrderService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Services - Frontend
builder.Services.AddScoped<IBrandFrontendService, BrandFrontendService>();
builder.Services.AddScoped<ICategoryFrontendService, CategoryFrontendService>();
builder.Services.AddScoped<IAddressFrontendService, AddressFrontendService>();
builder.Services.AddScoped<IOrderFrontendService, OrderFrontendService>();
builder.Services.AddScoped<ICartFrontendService, CartFrontendService>();
builder.Services.AddScoped<IProductFrontendService, ProductFrontendService>();
builder.Services.AddScoped<IUserFrontendService, UserFrontendService>();

//AutoMapper
/*builder.Services.AddScoped<ILoggerFactory, LoggerFactory>();
builder.Services.AddSingleton<IMapper>(
    serviceProvider =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            // Add your profiles or mapping configurations
            cfg.AddMaps(typeof(Program).Assembly);
        }, serviceProvider.GetService<ILoggerFactory>());

        return config.CreateMapper();
    });*/
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"]!;
}, typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization(); 

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
