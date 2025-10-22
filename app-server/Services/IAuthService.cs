using Studhub.Grpc.Data;
using StudHub.SharedDTO;

namespace Studhub.AppServer.Services;

public interface IAuthService
{
    Task<string?> ValidateUserAsync(string email, string password);
    Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(string email);
}