using System.Text.Json;
using Client.Services.StudUser;
using StudHub.SharedDTO.Users;

namespace Client.Services;

public class StudUserHttpClient : IStudUserClientService
{
    private readonly HttpClient _httpClient;

    public StudUserHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<CreateStudUserResponseDTO> CreateStudUser(CreateStudUserRequestDTO request)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync("StudUsers", request);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CreateStudUserResponseDTO>(response,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true })!;
    }
}