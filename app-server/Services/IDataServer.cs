using System.Threading;
using System.Threading.Tasks;
using Studhub.Grpc.Data;

namespace Studhub.AppServer.Services;

public interface IDataServer
{
    Task<CreateStudResponse> CreateStudAsync(CreateStudRequest req, CancellationToken ct = default);
    Task<GetStudByEmailResponse> GetStudByEmailAsync(GetStudByEmailRequest req, CancellationToken ct = default);
}