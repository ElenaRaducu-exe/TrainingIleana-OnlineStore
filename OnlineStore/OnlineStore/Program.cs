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

//Controllers
builder.Services.AddControllers(); 

// Services
builder.Services.AddScoped<ICreateUserService, CreateUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
