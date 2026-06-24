using Blazored.LocalStorage;
using HackZone.Web;
using HackZone.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("HackZone");

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "http://hackzone-api:5000";
builder.Services.AddHttpClient("HackZoneApi", c => c.BaseAddress = new Uri(apiBase));

builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
