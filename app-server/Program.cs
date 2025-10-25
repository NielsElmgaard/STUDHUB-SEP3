using Grpc.Net.Client;
using Studhub.Grpc.Data;
using Studhub.AppServer.Services;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// gRPC client for the Spring Boot Data Server
builder.Services.AddGrpcClient<StudService.StudServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["DataServer:GrpcAddress"]!);
});

builder.Services.AddScoped<IDataServer, GrpcDataServer>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

//Vi fjerner denne test senere!!!!!
builder.Services.AddHostedService<GrpcSmokeTest>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();