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
            throw new Exception(
                $"Error setting Brick Owl credentials (Status: {(int)httpResponse.StatusCode}). Response: {content.Substring(0, Math.Min(content.Length, 100))}");
        }

        var brickOwlResponse = await httpResponse.Content
            .ReadFromJsonAsync<BrickOwlCredentialsResponseDTO>();
        return brickOwlResponse!;    }
}