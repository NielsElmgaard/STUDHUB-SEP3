using System.Text.Json;
using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;

namespace Client.Services.StoreConnection;

public class StoreAuthHttpClient : IStoreAuthClientService
{
    private readonly HttpClient _httpClient;

    public StoreAuthHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BrickLinkCredentialsResponseDTO> SetBrickLinkCredentials(
        BrickLinkCredentialsRequestDTO credentialsRequest)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PutAsJsonAsync("auth/bricklink-connect",
                credentialsRequest);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResponse = await httpResponse.Content
                .ReadFromJsonAsync<BrickLinkCredentialsResponseDTO>();
            if (errorResponse != null &&
                !string.IsNullOrEmpty(errorResponse.ErrorMessage))
            {
                throw new Exception(
                    $"Failed to set BrickLink credentials: {errorResponse.ErrorMessage}");
            }

            string content = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("Detail",
                        out JsonElement detailElement) &&
                    detailElement.ValueKind == JsonValueKind.String)
                {
                    throw new Exception(detailElement.GetString());
                }
            }
            catch (JsonException)
            {
            }

            throw new Exception(
                $"Error setting BrickLink credentials (Status: {(int)httpResponse.StatusCode}). Response: {content.Substring(0, Math.Min(content.Length, 100))}");
        }

        var brickLinkResponse = await httpResponse.Content
            .ReadFromJsonAsync<BrickLinkCredentialsResponseDTO>();
        return brickLinkResponse!;
    }

    public async Task<BrickOwlCredentialsResponseDTO> SetBrickOwlCredentials(
        BrickOwlCredentialsRequestDTO credentialsRequest)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PutAsJsonAsync("auth/brickowl-connect",
                credentialsRequest);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResponse = await httpResponse.Content
                .ReadFromJsonAsync<BrickOwlCredentialsResponseDTO>();
            if (errorResponse != null &&
                !string.IsNullOrEmpty(errorResponse.ErrorMessage))
            {
                throw new Exception(
                    $"Failed to set Brick Owl credentials: {errorResponse.ErrorMessage}");
            }

            string content = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("Detail",
                        out JsonElement detailElement) &&
                    detailElement.ValueKind == JsonValueKind.String)
                {
                    throw new Exception(detailElement.GetString());
                }
            }
            catch (JsonException)
            {
            }

            throw new Exception(
                $"Server Error: Error setting Brick Owl credentials (Status: {(int)httpResponse.StatusCode}). Response: {content.Substring(0, Math.Min(content.Length, 100))}");
        }

        var brickOwlResponse = await httpResponse.Content
            .ReadFromJsonAsync<BrickOwlCredentialsResponseDTO>();
        return brickOwlResponse!;
    }

    public async Task<BrickLinkCredentialsResponseDTO>
        ClearBrickLinkCredentials(long studUserId)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.DeleteAsync(
                $"auth/bricklink-connect/{studUserId}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResponse = await httpResponse.Content
                .ReadFromJsonAsync<BrickLinkCredentialsResponseDTO>();
            if (errorResponse != null &&
                !string.IsNullOrEmpty(errorResponse.ErrorMessage))
            {
                throw new Exception(
                    $"Failed to clear BrickLink credentials: {errorResponse.ErrorMessage}");
            }

            string content = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("Detail",
                        out JsonElement detailElement) &&
                    detailElement.ValueKind == JsonValueKind.String)
                {
                    throw new Exception(detailElement.GetString());
                }
            }
            catch (JsonException)
            {
            }

            throw new Exception(
                $"Error clearing BrickLink credentials (Status: {(int)httpResponse.StatusCode}). Response: {content.Substring(0, Math.Min(content.Length, 100))}");
        }

        return new BrickLinkCredentialsResponseDTO { IsSucces = true };
    }

    public async Task<BrickOwlCredentialsResponseDTO> ClearBrickOwlCredentials(
        long studUserId)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.DeleteAsync(
                $"auth/brickowl-connect/{studUserId}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorResponse = await httpResponse.Content
                .ReadFromJsonAsync<BrickOwlCredentialsResponseDTO>();
            if (errorResponse != null &&
                !string.IsNullOrEmpty(errorResponse.ErrorMessage))
            {
                throw new Exception(
                    $"Failed to clear Brick Owl credentials: {errorResponse.ErrorMessage}");
            }

            string content = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("Detail",
                        out JsonElement detailElement) &&
                    detailElement.ValueKind == JsonValueKind.String)
                {
                    throw new Exception(detailElement.GetString());
                }
            }
            catch (JsonException)
            {
            }

            throw new Exception(
                $"Server Error: Error clearing Brick Owl credentials (Status: {(int)httpResponse.StatusCode}). Response: {content.Substring(0, Math.Min(content.Length, 100))}");
        }

        return new BrickOwlCredentialsResponseDTO { IsSucces = true };
    }

    public async Task<ConnectionStatusDTO?> IsBrickLinkConnected(
        long studUserId)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.GetAsync($"auth/bricklink-status/{studUserId}");


        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content
                .ReadFromJsonAsync<ConnectionStatusDTO>();
        }

        return new ConnectionStatusDTO
        {
            IsConnected = false,
            ErrorMessage =
                $"HTTP Error: {httpResponse.StatusCode} - {await httpResponse.Content.ReadAsStringAsync()}"
        };
    }

    public async Task<ConnectionStatusDTO?> IsBrickOwlConnected(long studUserId)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.GetAsync($"auth/brickowl-status/{studUserId}");

        if (httpResponse.IsSuccessStatusCode)
        {
            return await httpResponse.Content
                .ReadFromJsonAsync<ConnectionStatusDTO>();
        }

        return new ConnectionStatusDTO
        {
            IsConnected = false,
            ErrorMessage =
                $"HTTP Error: {httpResponse.StatusCode} - {await httpResponse.Content.ReadAsStringAsync()}"
        };
    }
}