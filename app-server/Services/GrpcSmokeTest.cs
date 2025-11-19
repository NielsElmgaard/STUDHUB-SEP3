using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Studhub.Grpc.Data;

namespace Studhub.AppServer.Services;
//DELETE når vi har lavet api
public sealed class GrpcSmokeTest : IHostedService
{
    private readonly ILogger<GrpcSmokeTest> _log;
    private readonly StudService.StudServiceClient _client;

    public GrpcSmokeTest(ILogger<GrpcSmokeTest> log, StudService.StudServiceClient client)
    { _log = log; _client = client; }

    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            _log.LogInformation("Calling gRPC…");

            var create = await _client.CreateStudAsync(new CreateStudRequest
            {
                Email = "alice@example.com",
                Username = "alice123",
                Password = "supersecret"
            }, cancellationToken: ct);
            _log.LogInformation("CreateStud → success={Success}, error='{Error}'",
                create.IsSuccess, create.ErrorMessage);

            var get = await _client.GetStudByIdAsync(new GetStudByIdRequest
            {
                Id = 1,
                Password = "supersecret"
            }, cancellationToken: ct);
            _log.LogInformation("GetStudById → username='{User}', error='{Error}'",
                get.Username, get.ErrorMessage);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "gRPC smoke test failed.");
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}