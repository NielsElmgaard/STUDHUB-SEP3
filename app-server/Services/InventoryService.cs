using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StudHub.SharedDTO;


namespace Studhub.AppServer.Services;

public class InventoryService : IInventoryService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    private static string url =
        "https://api.bricklink.com/api/store/v1/inventories";

    public InventoryService(IAuthService authService,
        HttpClient httpClient)
    {
        _authService = authService;
        _httpClient = httpClient;
    }

    public async Task<List<SetDTO>> GetUserSetsAsync(string email)
    {
        try
        {
            var queryParams = new Dictionary<string, string>
            {
                { "item_type", "SET" }
            };

            var jsonResponse =
                await ExecuteSignedApiCallAsync(email, queryParams);

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
                $"Error deserializing BrickLink response content for {email}",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during Set mapping or processing for {email}",
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
            if (unreservedChars.IndexOf(c)!=-1)
            {
                result.Append(c);
            }
            else
            {
                result.Append($"%{((int)c):X2}"); // formatted in Hexadecimal capitalized (2 digits guranteed)
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
            {"oauth_signature",oauthSignature},
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
    private async Task<string> ExecuteSignedApiCallAsync(string email,
        Dictionary<string, string> queryParams)
    {
        var credentials =
            await _authService.GetBrickLinkCredentialsAsync(email);
        if (credentials == null)
        {
            throw new Exception($"No BrickLink credentials for {email}");
        }


        string consumerKey = credentials.ConsumerKey;
        string consumerSecret = credentials.ConsumerSecret;
        string tokenValue = credentials.TokenValue;
        string tokenSecret = credentials.TokenSecret;

        try
        {
            // Full request url with query parameters
            var queryString = string.Join("&",
                queryParams.Select(kvp => $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));
            var fullUrl = $"{url}?{queryString}";

            // Auth header value
            var authHeaderValue = CreateOAuth1Header(
                url, "GET", queryParams, consumerKey, consumerSecret,
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
                $"HTTP or API Code Error during API call for {email}", e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An unexpected error occurred during API execution for {email}",
                e);
        }
    }
}