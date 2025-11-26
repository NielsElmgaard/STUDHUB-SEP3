using Grpc.Net.Client;
using Studhub.Grpc.Data;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Auth_Login;
using Studhub.AppServer.Services.StudUser;

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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IStudUserService, StudUserService>();


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