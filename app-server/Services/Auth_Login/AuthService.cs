using Grpc.Core;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;

namespace Studhub.AppServer.Services.Auth_Login;

public class AuthService : IAuthService
{
    private readonly StudService.StudServiceClient _grpcClient;
    private readonly HttpClient _httpClient;

    private static string brickLinkConnectionTestUrl =
        "https://api.bricklink.com/api/store/v1/items/PART/3001";

    public AuthService(StudService.StudServiceClient grpcClient,
        HttpClient httpClient)
    {
        _grpcClient = grpcClient;
        _httpClient = httpClient;
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

    public async Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(
        string email, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
        var connectionTest = await TestBrickLinkConnectionAsync(email,
            consumerKey, consumerSecret, tokenValue, tokenSecret);

        if (connectionTest == null || !connectionTest.IsValid)
        {
            throw new Exception(
                $"BrickLink credentials invalid: {connectionTest?.ErrorMessage ?? "Unknown error"}");
        }

        try
        {
            var grpcResponse = await _grpcClient.SetAuthByEmailAsync(
                new SetAuthByEmailRequest
                {
                    Email = email,
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    TokenValue = tokenValue,
                    TokenSecret = tokenSecret
                });

            if (!grpcResponse.IsSuccess)
            {
                throw new Exception(
                    $"gRPC SetAuthByEmailAsync save failed: {grpcResponse.ErrorMessage}");
            }

            return new BrickLinkCredentialsDTO
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                TokenValue = tokenValue,
                TokenSecret = tokenSecret
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"gRPC error: {e}");
            throw;
        }
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

    public async Task<BrickLinkConnectionTestDTO?> TestBrickLinkConnectionAsync(
        string email, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
        try
        {
            var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
                _httpClient, brickLinkConnectionTestUrl, HttpMethod.Get, consumerKey,
                consumerSecret, tokenValue, tokenSecret);

            return new BrickLinkConnectionTestDTO
            {
                IsValid = true,
                ErrorMessage = null
            };
        }
        catch (HttpRequestException e)
        {
            return new BrickLinkConnectionTestDTO
            {
                IsValid = false,
                ErrorMessage = $"HTTP/BrickLink error: {e.Message}"
            };
        }
        catch (Exception e)
        {
            return new BrickLinkConnectionTestDTO
            {
                IsValid = false,
                ErrorMessage = $"Unexpected error: {e.Message}"
            };
        }
    }


    public async Task<BrickOwlCredentialsResponseDTO?>
        SetBrickOwlCredentialsAsync(string email, string brickOwlApiKey)
    {
        throw new NotImplementedException();
    }

    public async Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(
        string email)
    {
        throw new NotImplementedException();
    }

    public async Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(
        string email, string brickOwlApiKey)
    {
        throw new NotImplementedException();
    }
}