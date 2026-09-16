using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using OnlineStore.Components;
using OnlineStore.Components.Pages;
using OnlineStore.Data;
using OnlineStore.DBModels;
using OnlineStore.JWTAuthentication.Handlers;
using OnlineStore.JWTAuthentication.Providers;
using OnlineStore.JWTAuthentication.Providers.Contracts;
using OnlineStore.JWTAuthentication.Services;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.Services;
using OnlineStore.Services.Contracts;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Razor components
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//HttpClient
builder.Services.AddHttpClient(); 

// Database
builder.Services.AddDbContext<OnlineStoreContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                )
            };
        });
builder.Services.AddAuthorization(); 
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
            provider.GetRequiredService<JWTAuthenticationStateProvider>());

builder.Services.AddScoped<JWTAuthorizationHandler>();
builder.Services.AddHttpClient("AuthenticatedUser").AddHttpMessageHandler<JWTAuthorizationHandler>();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();

//Controllers
builder.Services.AddControllers(); 

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICreateUserService, CreateUserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandsService, BrandsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<ICartProductsService, CartProductsService>(); 

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
