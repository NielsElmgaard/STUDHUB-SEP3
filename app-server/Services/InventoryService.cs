using System.Net.Http.Headers;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;

namespace Studhub.AppServer.Services;

public class InventoryService : IInventoryService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    private static string url =
        "https://api.bricklink.com/api/store/v1/inventory";

    public InventoryService(IAuthService authService,
        HttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    public async Task<List<SetDTO>> GetUserSetsAsync(string email)
    {
        var credentials =
            await _authService.GetBrickLinkCredentialsAsync(email);
        if (credentials == null)
        {
            throw new Exception("Could not get BrickLink credentials");
        }

        
        string consumerKey = credentials.ConsumerKey;
        string consumerSecret = credentials.ConsumerSecret;
        string tokenValue = credentials.TokenValue;
        string tokenSecret = credentials.TokenSecret;

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        return null; // TO DO
    }
}