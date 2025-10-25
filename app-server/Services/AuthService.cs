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
        var grpcResponse = await _grpcClient.GetStudByEmailAsync(
            new GetStudByEmailRequest
            {
                Email = email.Trim(),
                Password = password.Trim()
            });

        if (!string.IsNullOrEmpty(grpcResponse.ErrorMessage))
        {
            Console.WriteLine("Error grpc: " + grpcResponse.ErrorMessage);

            return null;
        }

        return grpcResponse.Username;
    }

    public async Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(
        string email)
    {
        try
        {
            var res = await _grpcClient.GetAuthByEmailAsync(
                new GetAuthByEmailRequest
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
                ConsumerKey = res.ConsumerKey,
                ConsumerSecret = res.ConsumerSecret,
                TokenValue = res.TokenValue,
                TokenSecret = res.TokenSecret
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}