using Grpc.Net.Client;
using Studhub.Grpc.Data;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Api_Auth;
using Studhub.AppServer.Services.Auth_Login;
using Studhub.AppServer.Services.Order;
using Studhub.AppServer.Services.StudUser;
using AppInv = Studhub.AppServer.Services.Inventory;
using Studhub.AppServer.Services.Lager;

using AppOrder = Studhub.AppServer.Services.Order;
using OrderClient = Studhub.Grpc.Data.OrderService.OrderServiceClient;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// gRPC client for the Spring Boot Data Server
builder.Services.AddGrpcClient<StudService.StudServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["DataServer:GrpcAddress"]!);
});
builder.Services.AddGrpcClient<InventoryService.InventoryServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["DataServer:GrpcAddress"]!);
});
builder.Services.AddGrpcClient<OrderClient>(o =>
{
    o.Address = new Uri(builder.Configuration["DataServer:GrpcAddress"]!);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
builder.Services.AddScoped<IInventoryService, AppInv.InventoryService>();
builder.Services.AddScoped<IStudUserService, StudUserService>();
builder.Services.AddScoped<IStudUserService, StudUserService>();
builder.Services.AddScoped<IOrderService, AppOrder.OrderService>();
builder.Services.AddScoped<StudhubLagerRepository>();
builder.Services.AddScoped<StudhubLagerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


app.UseAuthorization();

app.MapControllers();


app.Run();