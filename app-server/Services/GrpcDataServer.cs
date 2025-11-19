using System.Threading;
using System.Threading.Tasks;
using Studhub.Grpc.Data;

namespace Studhub.AppServer.Services;

public sealed class GrpcDataServer : IDataServer
{
    private readonly StudService.StudServiceClient _client;

    public GrpcDataServer(StudService.StudServiceClient client) => _client = client;

    public Task<CreateStudResponse> CreateStudAsync(CreateStudRequest req, CancellationToken ct = default)
        => _client.CreateStudAsync(req, cancellationToken: ct).ResponseAsync;

    public Task<GetStudByIdResponse> GetStudByIdAsync(GetStudByIdRequest req, CancellationToken ct = default)
        => _client.GetStudByIdAsync(req, cancellationToken: ct).ResponseAsync;
}