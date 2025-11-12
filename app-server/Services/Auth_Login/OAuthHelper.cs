using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Studhub.AppServer.Services;

public class OAuthHelper
{
    // Encode string according to RFC 3986
    public static string UrlEncode(string value)
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


    public static string CreateOAuth1Header(
        string baseUrl,
        string httpMethod,
        Dictionary<string, string>? queryParams,
        string consumerKey,
        string consumerSecret,
        string tokenValue,
        string tokenSecret)
    {
        var oauthSignatureMethod = "HMAC-SHA1";

        var oauthTimestamp = new DateTimeOffset(DateTime.UtcNow)
            .ToUnixTimeSeconds()
            .ToString();
        var oauthNonce = Guid.NewGuid().ToString("N");
        var oauthVersion = "1.0";

        var allParams = new SortedDictionary<string, string>
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
                allParams[kvp.Key] = kvp.Value;
            }
        }

        // Create oauthSignature
        var parameterString = string.Join("&",
            allParams.Select(kvp =>
                $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));

        var signatureBaseString =
            $"{httpMethod.ToUpper()}&{UrlEncode(baseUrl)}&{UrlEncode(parameterString)}";

        var signingKey =
            $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";

        using var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
        var hash =
            hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
        var oauthSignature = Convert.ToBase64String(hash);


        // Add oauth signature to paramters
        allParams["oauth_signature"] = oauthSignature;


        // Create authentication header value
        var authHeaderValue = "realm=\"\", " + string.Join(", ",
            allParams.Select(kvp =>
                $"{UrlEncode(kvp.Key)}=\"{UrlEncode(kvp.Value)}\""));


        return authHeaderValue;
    }


    // OAuth 1.0a Helper Method
    public static async Task<string> ExecuteSignedApiCallAsync(
        HttpClient httpClient,
        string url,
        HttpMethod method,
        string consumerKey,
        string consumerSecret,
        string tokenValue,
        string tokenSecret,
        Dictionary<string, string>? queryParams = null,
        string? jsonBody = null)
    {
        var requestUrl = url;

        if (queryParams != null && queryParams.Count > 0)
        {
            requestUrl += "?" + string.Join("&", queryParams.Select(kvp =>
                $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));
        }

        // Auth header value
        var authHeaderValue = CreateOAuth1Header(url, method.Method,
            queryParams, consumerKey, consumerSecret, tokenValue,
            tokenSecret);

        // applying header to request
        var request = new HttpRequestMessage(method, requestUrl);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("OAuth", authHeaderValue);
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                "application/json"));

        if (jsonBody != null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8,
                "application/json");

        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(request);
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException("HTTP request failed", e);
        }
        catch (Exception e)
        {
            throw new Exception("An unexpected error occurred during HTTP request", e);
        }

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        // Check for BrickLink API-level errors
        CheckApiResponseForError(jsonResponse);

        return jsonResponse;
    }

    private static void CheckApiResponseForError(string jsonResponse)
    {
        using var document = JsonDocument.Parse(jsonResponse);
        var root = document.RootElement;

        if (root.TryGetProperty("meta", out JsonElement meta) &&
            meta.TryGetProperty("code", out JsonElement code) &&
            code.GetInt32() != 200)
        {
            var message = meta.TryGetProperty("message", out JsonElement msg)
                ? msg.GetString()
                : "Unknown API Error";
            throw new HttpRequestException(
                $"BrickLink API Error: {message} (Code: {code.GetInt32()})");
        }
    }
}