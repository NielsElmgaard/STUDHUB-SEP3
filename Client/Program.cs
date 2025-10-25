using Client.Components;
using Client.Services;
using Microsoft.AspNetCore.Components;
using Studhub.AppServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

    builder.Services.AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5299/")
    });

builder.Services.AddScoped<ILoginAuthService, LoginAuthService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<IInventoryService, InventoryServiceClientProxy>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();


app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
