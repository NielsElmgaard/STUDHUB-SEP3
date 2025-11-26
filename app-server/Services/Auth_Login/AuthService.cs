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

    private static string brickOwlConnectionTestUrl =
        "https://api.brickowl.com/v1/user/details";

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
        var connectionTest = await TestBrickLinkConnectionAsync(consumerKey,
            consumerSecret, tokenValue, tokenSecret);

        if (connectionTest == null || !connectionTest.IsValid)
        {
            string errorMessage =
                connectionTest?.ErrorMessage ??
                "Invalid or expired BrickLink credentials (Code: 401).";
            throw new ArgumentException(
                $"BrickLink credentials invalid: {errorMessage}");
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
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            throw new InvalidOperationException(
                "Connection could not be established.", e);
        }
        catch (Exception e)
        {
            Console.WriteLine($"gRPC Error: {e}");
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
        string consumerKey,
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


    public async Task<BrickOwlCredentialsDTO?>
        SetBrickOwlCredentialsAsync(long studUserId, string brickOwlApiKey)
    {
        var connectionTest = await TestBrickOwlConnectionAsync(brickOwlApiKey);

        if (connectionTest == null || !connectionTest.IsValid)
        {
            string errorMessage =
                connectionTest?.ErrorMessage ??
                "Invalid or expired Brick Owl credentials (Code: 401).";
            throw new ArgumentException(
                $"Brick Owl credentials invalid: {errorMessage}");
        }

        try
        {
            var grpcResponse = await _grpcClient.SetBrickOwlAuthByIdAsync(
                new SetBrickOwlAuthByIdRequest()
                {
                    Id = studUserId,
                    ApiKey = brickOwlApiKey
                });

            if (!grpcResponse.IsSuccess)
            {
                throw new Exception(
                    $"gRPC SetBrickOwlAuthByIdAsync save failed: {grpcResponse.ErrorMessage}");
            }

            return new BrickOwlCredentialsDTO
            {
                BrickOwlApiKey = brickOwlApiKey
            };
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            throw new InvalidOperationException(
                "Connection could not be established.", e);
        }
        catch (Exception e)
        {
            Console.WriteLine($"gRPC error: {e}");
            throw;
        }
    }

    public async Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(
        long studUserId)
    {
        try
        {
            var res = await _grpcClient.GetBrickOwlAuthByIdAsync(
                new GetBrickOwlAuthByIdRequest()
                {
                    Id = studUserId
                });
            var noCreds = string.IsNullOrEmpty(res.ApiKey);

            if (noCreds) return null;

            return new BrickOwlCredentialsDTO()
            {
                BrickOwlApiKey = res.ApiKey
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(
        string brickOwlApiKey)
    {
        try
        {
            var request =
                new HttpRequestMessage(HttpMethod.Get,
                    $"{brickOwlConnectionTestUrl}?key={brickOwlApiKey}");
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return new BrickOwlConnectionTestDTO()
            {
                IsValid = true,
                ErrorMessage = null
            };
        }
        catch (HttpRequestException e)
        {
            return new BrickOwlConnectionTestDTO()
            {
                IsValid = false,
                ErrorMessage = $"HTTP/BrickOwl error: {e.Message}"
            };
        }
        catch (Exception e)
        {
            return new BrickOwlConnectionTestDTO()
            {
                IsValid = false,
                ErrorMessage = $"Unexpected error: {e.Message}"
            };
        }
    }

    public async Task<BrickLinkCredentialsDTO?> ClearBrickLinkCredentialsAsync(
        long studUserId, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
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
                    $"gRPC ClearBrickLinkAuthByIdAsync save failed: {grpcResponse.ErrorMessage}");
            }

            return null;
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            throw new InvalidOperationException(
                "Connection could not be established.", e);
        }
        catch (Exception e)
        {
            Console.WriteLine($"gRPC Error: {e}");
            throw;
        }
    }

    public async Task<BrickOwlCredentialsDTO?>
        ClearBrickOwlCredentialsAsync(long studUserId, string brickOwlApiKey)
    {
        try
        {
            var grpcResponse = await _grpcClient.SetBrickOwlAuthByIdAsync(
                new SetBrickOwlAuthByIdRequest()
                {
                    Id = studUserId,
                    ApiKey = brickOwlApiKey
                });

            if (!grpcResponse.IsSuccess)
            {
                throw new Exception(
                    $"gRPC ClearBrickOwlAuthByIdAsync save failed: {grpcResponse.ErrorMessage}");
            }

            return null;
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            throw new InvalidOperationException(
                "Connection could not be established.", e);
        }
        catch (Exception e)
        {
            Console.WriteLine($"gRPC error: {e}");
            throw;
        }
    }

    public async Task<bool> IsBrickLinkConnectedAsync(long studUserId)
    {
        var request = new GetBrickLinkAuthByIdRequest
        {
            Id = studUserId
        };

        try
        {
            GetBrickLinkAuthByIdResponse response =
                await _grpcClient.GetBrickLinkAuthByIdAsync(request);
            return
                !string.IsNullOrEmpty(response
                    .ConsumerKey); // If Consumer Key is not empty (assumes the other api keys is empty as well), the user is connected.
        }
        catch (RpcException e)
        {
            Console.WriteLine(
                $"gRPC Error calling GetBrickLinkAuthById: {e.Status.Detail}");
            return false;
        }
    }

    public async Task<bool> IsBrickOwlConnectedAsync(long studUserId)
    {
        var request = new GetBrickOwlAuthByIdRequest
        {
            Id = studUserId
        };

        try
        {
            GetBrickOwlAuthByIdResponse response =
                await _grpcClient.GetBrickOwlAuthByIdAsync(request);
            return
                !string.IsNullOrEmpty(response
                    .ApiKey); // If Api Key is not empty, the user is connected.
        }
        catch (RpcException e)
        {
            Console.WriteLine(
                $"gRPC Error calling GetBrickOwlAuthById: {e.Status.Detail}");
            return false;
        }
    }
}