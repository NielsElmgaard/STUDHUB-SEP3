using System.Text.Json;
using Studhub.AppServer.Services.Auth_Login;
using StudHub.SharedDTO;

namespace Studhub.AppServer.Services.Api_Auth;

public class ApiAuthService : IApiAuthService
{
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;

    public ApiAuthService(IAuthService authService, HttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    public async Task<List<T>> GetBrickLinkResponse<T>(int studUserId, string brickLinkUrl, Dictionary<string, string>? queryParams = null)
    {
        var credentials = await _authService.GetBrickLinkCredentialsAsync(studUserId);
        if (credentials == null)
        {
            throw new Exception($"No BrickLink credentials for {studUserId}");
        }

        var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
            _httpClient,
            brickLinkUrl,
            HttpMethod.Get,
            credentials.ConsumerKey,
            credentials.ConsumerSecret,
            credentials.TokenValue,
            credentials.TokenSecret,
            queryParams);
        
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        
        var brickLinkResponse =
            JsonSerializer
                .Deserialize<
                    BrickLinkApiResponse<List<T>>>(
                    jsonResponse, options);

        if (brickLinkResponse?.Data == null)
        {
            return new List<T>();
        }

        return brickLinkResponse.Data;
    }

    public async Task<List<T>> GetBrickOwlResponse<T>(int studUserId)
    {
        throw new NotImplementedException();
    }
}