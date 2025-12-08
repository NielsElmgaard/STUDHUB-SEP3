using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Studhub.AppServer.Services.Api_Auth;
using Studhub.AppServer.Services.Auth_Login;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;
using InventoryClient = Studhub.Grpc.Data.InventoryService.InventoryServiceClient;

namespace Studhub.AppServer.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly IApiAuthService _apiAuthService;
    private readonly InventoryClient _inventoryClient;

    private static string brickLinkInventoriesUrl =
        "https://api.bricklink.com/api/store/v1/inventories";

    private static string brickOwlInventoriesUrl =
        "https://api.brickowl.com/v1/inventory/list";

    public InventoryService(IAuthService authService,
        HttpClient httpClient, IApiAuthService apiAuthService, InventoryClient inventoryClient)
    {
        _authService = authService;
        _httpClient = httpClient;
        _apiAuthService = apiAuthService;
        _inventoryClient = inventoryClient;
    }

    public async Task<List<SetDTO>> GetUserSetsAsync(int studUserId)
    {
        try
        {
            var queryParams = new Dictionary<string, string>
            {
                { "item_type", "SET" }
            };

            var jsonResponse =
                await ExecuteSignedApiCallAsync(studUserId, queryParams);

            // Deserialize response
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            var brickLinkResponse =
                JsonSerializer
                    .Deserialize<
                        BrickLinkApiResponse<List<BrickLinkInventoryItem>>>(
                        jsonResponse, options);

            if (brickLinkResponse?.Data == null)
            {
                return new List<SetDTO>();
            }

            // Map Internal Models/BrickLink DTOs to shared SetDTO
            var sets = brickLinkResponse.Data.Select(item => new SetDTO
            {
                ItemNumber = item.ItemInfo.Number,
                Name = WebUtility.HtmlDecode(item.ItemInfo.Name),
                Quantity = item.Quantity,
                Condition = item.Condition,
                PriceString = item.Price.ToString(CultureInfo.InvariantCulture),
                Completeness = item.Completeness
            }).ToList();

            return sets;
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
                $"An error occurred during Set mapping or processing for {studUserId}",
                e);
        }
    }

    public async Task<List<BrickLinkInventoryDTO>>
        GetUserBrickLinkInventoryAsync(
            int studUserId)
    {

        return await _apiAuthService.GetBrickLinkResponse<BrickLinkInventoryDTO>(studUserId, brickLinkInventoriesUrl);
    }

    public async Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(
        int studUserId)
    {
        return await _apiAuthService.GetBrickOwlResponse<BrickOwlLotDTO>(studUserId, brickOwlInventoriesUrl);
    }

    public async Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(
        int studUserId)
    {
        var credentials =
            await _authService.GetBrickOwlCredentialsAsync(studUserId);
        if (credentials == null)
        {
            throw new Exception($"No Brick Owl credentials for {studUserId}");
        }

        var fullUrl =
            $"{brickOwlInventoriesUrl}?key={credentials.BrickOwlApiKey}";
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var uniqueKeys = new HashSet<string>();

        try
        {
            var lotElements =
                JsonSerializer.Deserialize<JsonElement[]>(jsonResponse);

            if (lotElements == null || lotElements.Length == 0)
            {
                return new List<string>();
            }

            var firstLot = lotElements[0];

            if (firstLot.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException(
                    "First element in inventory array is not a JSON object.");
            }

            foreach (var property in firstLot.EnumerateObject())
            {
                uniqueKeys.Add(property.Name);
            }

            return uniqueKeys.ToList();
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error parsing Brick Owl response content for key discovery for {studUserId}. Check if JSON is valid.",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during key discovery for {studUserId}",
                e);
        }
    }

    public async Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(int studUserId)
    {
var credentials = await _authService.GetBrickLinkCredentialsAsync(studUserId);
    if (credentials == null)
    {
        throw new Exception($"No BrickLink credentials for {studUserId}");
    }
    
    var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
        _httpClient, brickLinkInventoriesUrl, HttpMethod.Get,
        credentials.ConsumerKey,
        credentials.ConsumerSecret, credentials.TokenValue,
        credentials.TokenSecret);
    
    var uniqueKeys = new HashSet<string>();

    try
    {
        using (JsonDocument document = JsonDocument.Parse(jsonResponse))
        {
            var root = document.RootElement;
            
            if (!root.TryGetProperty("data", out JsonElement dataElement) || 
                dataElement.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException("BrickLink response 'data' property is missing or not an array.");
            }

            if (dataElement.GetArrayLength() == 0)
            {
                return new List<string>();
            }

            var firstLot = dataElement[0];

            if (firstLot.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("First element in data array is not a JSON object.");
            }

            foreach (var property in firstLot.EnumerateObject())
            {
                uniqueKeys.Add(property.Name);
            }
        }
        
        return uniqueKeys.ToList();
    }
    catch (JsonException e)
    {
        throw new JsonException(
            $"Error parsing BrickLink response content for key discovery for {studUserId}. Check JSON structure.",
            e);
    }
    catch (Exception e)
    {
        throw new Exception(
            $"An error occurred during key discovery for {studUserId}",
            e);
    }
}


    // Encode string according to RFC 3986
    private string UrlEncode(string value)
    {
        const string unreservedChars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~"; //  RFC396: unreserved  = ALPHA / DIGIT / "-" / "." / "_" / "~"
        var result = new StringBuilder();
        foreach (char c in value)
        {
            if (unreservedChars.IndexOf(c) != -1)
            {
                result.Append(c);
            }
            else
            {
                result.Append(
                    $"%{((int)c):X2}"); // formatted in Hexadecimal capitalized (2 digits guranteed)
            }
        }

        return result.ToString();
    }


    private string CreateOAuth1Header(string baseUrl, string method,
        Dictionary<string, string>? queryParams, string consumerKey,
        string consumerSecret, string tokenValue, string tokenSecret)
    {
        var oauthSignatureMethod = "HMAC-SHA1";

        var oauthTimestamp = new DateTimeOffset(DateTime.UtcNow)
            .ToUnixTimeSeconds()
            .ToString();
        var oauthNonce = Guid.NewGuid().ToString("N");
        var oauthVersion = "1.0";

        var parametersForSignature = new SortedDictionary<string, string>
        {
            { "oauth_consumer_key", consumerKey },
            { "oauth_token", tokenValue },
            { "oauth_signature_method", oauthSignatureMethod },
            { "oauth_timestamp", oauthTimestamp },
            { "oauth_nonce", oauthNonce },
            { "oauth_version", oauthVersion }
        };

        if (queryParams != null)
        {
            foreach (var kvp in queryParams)
            {
                parametersForSignature.Add(kvp.Key, kvp.Value);
            }
        }

        // Create oauthSignature
        var parameterString = string.Join("&",
            parametersForSignature.Select(kvp =>
                $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));

        var signatureBaseString =
            $"{method.ToUpper()}&{UrlEncode(baseUrl)}&{UrlEncode(parameterString)}";

        var signingKey =
            $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";

        string oauthSignature;
        using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey)))
        {
            byte[] hash =
                hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
            oauthSignature = Convert.ToBase64String(hash);
        }

        var authParamsForHeader = new SortedDictionary<string, string>
        {
            { "oauth_consumer_key", consumerKey },
            { "oauth_token", tokenValue },
            { "oauth_signature", oauthSignature },
            { "oauth_signature_method", oauthSignatureMethod },
            { "oauth_timestamp", oauthTimestamp },
            { "oauth_nonce", oauthNonce },
            { "oauth_version", oauthVersion }
        };

        // Create authentication header value
        var authHeaderValue = "realm=\"\", " + string.Join(", ",
            authParamsForHeader.Select(kvp =>
                $"{UrlEncode(kvp.Key)}=\"{UrlEncode(kvp.Value)}\""));


        return authHeaderValue;
    }


    // OAuth 1.0a Helper Method
    private async Task<string> ExecuteSignedApiCallAsync(int studUserId,
        Dictionary<string, string> queryParams)
    {
        var credentials =
            await _authService.GetBrickLinkCredentialsAsync(studUserId);
        if (credentials == null)
        {
            throw new Exception($"No BrickLink credentials for {studUserId}");
        }


        string consumerKey = credentials.ConsumerKey;
        string consumerSecret = credentials.ConsumerSecret;
        string tokenValue = credentials.TokenValue;
        string tokenSecret = credentials.TokenSecret;

        try
        {
            // Full request brickLinkInventoriesUrl with query parameters
            var queryString = string.Join("&",
                queryParams.Select(kvp =>
                    $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));
            var fullUrl = $"{brickLinkInventoriesUrl}?{queryString}";

            // Auth header value
            var authHeaderValue = CreateOAuth1Header(
                brickLinkInventoriesUrl, "GET", queryParams, consumerKey,
                consumerSecret,
                tokenValue, tokenSecret);

            // applying header to request
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("OAuth", authHeaderValue);
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);


            response
                .EnsureSuccessStatusCode(); // if IsSuccessStatusCode=false -> throws exception

            var jsonResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine("===== BrickLink Response Body =====");
            Console.WriteLine(jsonResponse);


            // To deserialize only get meta and code response for api succes check before full deserialization
            using (JsonDocument document = JsonDocument.Parse(jsonResponse))
            {
                var root = document.RootElement;

                if (root.TryGetProperty("meta", out JsonElement meta) &&
                    meta.TryGetProperty("code", out JsonElement code) &&
                    code.GetInt32() != 200)
                {
                    var message =
                        meta.TryGetProperty("message", out JsonElement msg)
                            ? msg.GetString()
                            : "Unknown API Error";
                    throw new HttpRequestException(
                        $"BrickLink API Error: {message} (Code: {code.GetInt32()})");
                }
            }

            return jsonResponse;
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException(
                $"HTTP or API Code Error during API call for {studUserId}", e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An unexpected error occurred during API execution for {studUserId}",
                e);
        }
    }

    public async Task<UpdateResponse> UpdateBrickLinkInventoryAsync(int studUserId, List<BrickLinkInventoryDTO> inventories)
    {
        var request = new UpdateRequest()
        {
            UserId = studUserId,
            Source = DataSource.Bricklink
        };
        foreach (var inv in inventories)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add((Struct.Parser.ParseJson(invJson)));
        }
        return 
            await _inventoryClient.SetInventoriesAsync(request);
    }
}