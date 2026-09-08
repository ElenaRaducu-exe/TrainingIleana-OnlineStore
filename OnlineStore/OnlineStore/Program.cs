using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using OnlineStore.Components;
using OnlineStore.DBModels;
using OnlineStore.Services.Contracts;
using OnlineStore.Services;
using OnlineStore.JWTAuthentication.Services.Contracts;
using OnlineStore.JWTAuthentication.Services;
using OnlineStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineStore.JWTAuthentication.Handlers;

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
builder.Services.AddAuthorization();

/*builder.Services.AddScoped<JWTAuthorizationHandler>();
builder.Services.AddHttpClient("AuthenticatedClient",
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7141/");
    }).AddHttpMessageHandler<JWTAuthorizationHandler>(); */

//Controllers
builder.Services.AddControllers(); 

// Services
builder.Services.AddScoped<ICreateUserService, CreateUserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUsersService, UsersService>();

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
