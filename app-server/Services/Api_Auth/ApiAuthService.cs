using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
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

    public async Task<List<T>> GetBrickLinkResponse<T>(int studUserId, string url,
        Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var credentials = await _authService.GetBrickLinkCredentialsAsync(studUserId);
            if (credentials == null) throw new Exception($"No BrickLink credentials for {studUserId}");

            var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
                _httpClient,
                url,
                HttpMethod.Get,
                credentials.ConsumerKey,
                credentials.ConsumerSecret,
                credentials.TokenValue,
                credentials.TokenSecret,
                queryParams);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var brickLinkResponse =
                JsonSerializer
                    .Deserialize<
                        BrickLinkApiResponse<List<T>>>(
                        jsonResponse, options);

            if (brickLinkResponse?.Data == null) return new List<T>();

            return brickLinkResponse.Data;
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error deserializing BrickLink response content for {studUserId}",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during fetching order from BrickLink for {studUserId}",
                e);
        }
    }

    public async Task<List<T>> GetBrickOwlResponse<T>(int studUserId, string url,
        Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var credentials =
                await _authService.GetBrickOwlCredentialsAsync(studUserId);
            if (credentials == null)
                throw new Exception(
                    $"No Brick Owl credentials for {studUserId}");

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["key"] = credentials.BrickOwlApiKey;
            if (queryParams != null)
                foreach (var param in queryParams)
                    query[param.Key] = param.Value;

            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();


            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<T>>(
                jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<T>();
            ;
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error deserializing Brick Owl response content for {studUserId}",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during processing Brick Owl inventories for {studUserId}",
                e);
        }
    }

    public async Task<T> PostBrickOwlResponse<T>(int studUserId, string url, Dictionary<string, string> formData)
    {
        try
        {
            var credentials =
                await _authService.GetBrickOwlCredentialsAsync(studUserId);
            if (credentials == null)
                throw new Exception(
                    $"No Brick Owl credentials for {studUserId}");

            formData.Add("key", credentials.BrickOwlApiKey);
            using var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(url, content);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("response: " + jsonResponse);
            response.EnsureSuccessStatusCode();


            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var brickOwlResponse = JsonSerializer.Deserialize<T>(jsonResponse, options)
                                   ?? throw new JsonException("Error deserializing Brick Owl post response content");

            return brickOwlResponse;
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error deserializing Brick Owl response content for {studUserId}",
                e);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Console.WriteLine("Stack Trace: " + e.StackTrace);
            throw new Exception(
                $"An error occurred during processing Brick Owl inventories for {studUserId}",
                e);
        }
    }
}