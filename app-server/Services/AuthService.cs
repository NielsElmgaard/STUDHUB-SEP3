using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using Grpc.Core;

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

    public async Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(string email)
    {
        try
        {
            var res = await _grpcClient.GetAuthByEmailAsync(new GetAuthByEmailRequest
            {
                Email = email
            });
            var noCreds =
                string.IsNullOrEmpty(res.ConsumerKey) &&
                string.IsNullOrEmpty(res.ConsumerSecret) &&
                string.IsNullOrEmpty(res.TokenValue) &&
                string.IsNullOrEmpty(res.TokenSecret);

            if (noCreds) return null;

            return new BrickLinkCredentialsDTO
            {
                ConsumerKey    = res.ConsumerKey,
                ConsumerSecret = res.ConsumerSecret,
                TokenValue     = res.TokenValue,
                TokenSecret    = res.TokenSecret
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}