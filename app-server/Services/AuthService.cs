using Studhub.Grpc.Data;
using StudHub.SharedDTO;

namespace Studhub.AppServer.Services;

public class AuthService : IAuthService
{
    private readonly StudService.StudServiceClient _grpcClient;

    public AuthService(StudService.StudServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<string?> ValidateUserAsync(string email, string password)
    {
        var request = new GetStudByEmailRequest()
        {
            Email = email,
            Password = password
        };


        var response = await _grpcClient.GetStudByEmailAsync(request);
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            Console.WriteLine("Error grpc: " + response.ErrorMessage);

            return null;
        }

        return response.Username;
    }

    public async Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(
        string email)
    {
        throw new NotImplementedException();
    }
}