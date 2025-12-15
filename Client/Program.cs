using Client.Components;
using Client.Components.Authentication;
using Client.Services;
using Client.Services.StoreConnection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Studhub.AppServer.Services;
using Client.Services;
using Client.Services.Auth_Login;
using Client.Services.Inventory;
using Client.Services.StudUser;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

    builder.Services.AddScoped(sp => new HttpClient
    {
        //BaseAddress = new Uri("https://localhost:7245/")
        BaseAddress = new Uri("http://localhost:5299/")
    });

builder.Services.AddSingleton<Client.Services.Dashboard.DashboardRegistry>();
builder.Services.AddScoped<ILoginClientService, LoginClientHttpClient>();
builder.Services.AddScoped<IInventoryClientService, InventoryHttpClient>();
builder.Services.AddScoped<IStudUserClientService, StudUserHttpClient>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthProvider>();
builder.Services.AddScoped<IStoreAuthClientService, StoreAuthHttpClient>();
builder.Services.AddAuthentication();
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<SimpleAuthProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthProvider>());
builder.Services.AddScoped<Client.Services.Dashboard.DashboardSettingsService>();

var app = builder.Build();

// Configure the HTTP userRequest pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();


//app.MapRazorPages();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
