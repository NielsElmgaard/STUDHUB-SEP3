using Grpc.Core;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;
using StudHub.SharedDTO.Users;

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

    public async Task<StudUserDTO?> ValidateUserAsync(string email,
        string password)
    {
        var grpcResponse = await _grpcClient.GetStudByEmailAsync(
            new GetStudByEmailRequest()
            {
                Email = email,
                Password = password.Trim()
            });

        if (!string.IsNullOrEmpty(grpcResponse.ErrorMessage))
        {
            Console.WriteLine("Error grpc: " + grpcResponse.ErrorMessage);

            return null;
        }

        return new StudUserDTO
        {
            Id = grpcResponse.Id,
            Email = grpcResponse.Email,
            Username = grpcResponse.Username
        };
    }

    public async Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(
        long studUserId, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
        var connectionTest = await TestBrickLinkConnectionAsync(studUserId,
            consumerKey, consumerSecret, tokenValue, tokenSecret);

        if (connectionTest == null || !connectionTest.IsValid)
        {
            throw new Exception(
                $"BrickLink credentials invalid: {connectionTest?.ErrorMessage ?? "Unknown error"}");
        }

        try
        {
            var grpcResponse = await _grpcClient.SetBrickLinkAuthByIdAsync(
                new SetBrickLinkAuthByIdRequest()
                {
                    Id = studUserId,
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    TokenValue = tokenValue,
                    TokenSecret = tokenSecret
                });

            if (!grpcResponse.IsSuccess)
            {
                throw new Exception(
                    $"gRPC SetBrickLinkAuthByIdAsync save failed: {grpcResponse.ErrorMessage}");
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
        long studUserId)
    {
        try
        {
            var res = await _grpcClient.GetBrickLinkAuthByIdAsync(
                new GetBrickLinkAuthByIdRequest
                {
                    Id = studUserId
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
        long studUserId, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
        try
        {
            var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
                _httpClient, brickLinkConnectionTestUrl, HttpMethod.Get,
                consumerKey,
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
        SetBrickOwlCredentialsAsync(long studUserId, string brickOwlApiKey)
    {
        throw new NotImplementedException();
    }

    public async Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(
        long studUserId)
    {
        throw new NotImplementedException();
    }

    public async Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(
        long studUserId, string brickOwlApiKey)
    {
        throw new NotImplementedException();
    }
}