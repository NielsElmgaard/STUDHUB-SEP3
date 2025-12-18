using Grpc.Core;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;
using StudHub.SharedDTO.Users;
using Microsoft.Extensions.Configuration;

namespace Studhub.AppServer.Services.Auth_Login;

public class AuthService : IAuthService
{
    private readonly StudService.StudServiceClient _grpcClient;
    private readonly HttpClient _httpClient;

    private readonly string brickLinkConnectionTestUrl =
        "https://api.bricklink.com/api/store/v1/items/PART/3001";

    private readonly string brickOwlConnectionTestUrl =
        "https://api.brickowl.com/v1/user/details";

    public AuthService(StudService.StudServiceClient grpcClient,
        HttpClient httpClient,IConfiguration config)
    {
        _grpcClient = grpcClient;
        _httpClient = httpClient;
        
        
        // Til IntegrationsTest
        brickLinkConnectionTestUrl = config["ApiUrls:BrickLinkTest"] 
                                     ?? "https://api.bricklink.com/api/store/v1/items/PART/3001";
            
        brickOwlConnectionTestUrl = config["ApiUrls:BrickOwlTest"] 
                                    ?? "https://api.brickowl.com/v1/user/details";
    }

    public async Task<StudUserDTO?> ValidateUserAsync(string email,
        string password)
    {
        try
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
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            throw new InvalidOperationException(
                "Could not establish connection to the data service. Please try again later.",
                e);
        }
    }

    public async Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(
        int studUserId, string consumerKey,
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
        int studUserId)
    {
        try
        {
            var res = await _grpcClient.GetBrickLinkAuthByIdAsync(
                new UserId
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
        SetBrickOwlCredentialsAsync(int studUserId, string brickOwlApiKey)
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
        int studUserId)
    {
        try
        {
            var res = await _grpcClient.GetBrickOwlAuthByIdAsync(
                new UserId()
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
        int studUserId, string consumerKey,
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
        ClearBrickOwlCredentialsAsync(int studUserId, string brickOwlApiKey)
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

    public async Task<ConnectionStatusDTO> IsBrickLinkConnectedAsync(
        int studUserId)
    {
        var request = new UserId
        {
            Id = studUserId
        };

        try
        {
            GetBrickLinkAuthByIdResponse response =
                await _grpcClient.GetBrickLinkAuthByIdAsync(request);

            bool isConnected =
                !string.IsNullOrEmpty(response
                    .ConsumerKey); // If Consumer Key is not empty (assumes the other api keys is empty as well), the user is connected.

            return new ConnectionStatusDTO
            {
                IsConnected = isConnected,
                ErrorMessage = null
            };
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            Console.WriteLine(
                $"gRPC Error calling GetBrickLinkAuthById: {e.Status.Detail}");
            return new ConnectionStatusDTO
            {
                IsConnected = false,
                ErrorMessage = $"gRPC Error: {e.Status.Detail}"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected Error: {e.Message}");
            return new ConnectionStatusDTO
            {
                IsConnected = false,
                ErrorMessage = $"Unexpected Error: {e.Message}"
            };
        }
    }

    public async Task<ConnectionStatusDTO> IsBrickOwlConnectedAsync(
        int studUserId)
    {
        var request = new UserId
        {
            Id = studUserId
        };

        try
        {
            GetBrickOwlAuthByIdResponse response =
                await _grpcClient.GetBrickOwlAuthByIdAsync(request);
            bool isConnected =
                !string.IsNullOrEmpty(response
                    .ApiKey); // If Api Key is not empty, the user is connected.

            return new ConnectionStatusDTO
            {
                IsConnected = isConnected,
                ErrorMessage = null
            };
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
        {
            Console.WriteLine(
                $"gRPC Error calling GetBrickOwlAuthById: {e.Status.Detail}");
            return new ConnectionStatusDTO
            {
                IsConnected = false,
                ErrorMessage = $"gRPC Error: {e.Status.Detail}"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected Error: {e.Message}");
            return new ConnectionStatusDTO
            {
                IsConnected = false,
                ErrorMessage = $"Unexpected Error: {e.Message}"
            };
        }
    }
}