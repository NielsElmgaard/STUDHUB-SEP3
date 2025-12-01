using System.Text.Json;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Users;

namespace Client.Services.Auth_Login;

public class LoginClientHttpClient : ILoginClientService
{
    private readonly HttpClient _httpClient;

    public LoginClientHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StudUserDTO> LoginUserAsync(LoginRequestDTO request)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PostAsJsonAsync("auth/login", request);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {error}");
        }

        LoginResponseDTO? body;
        try
        {
            string content = await httpResponse.Content.ReadAsStringAsync();
            body = JsonSerializer.Deserialize<LoginResponseDTO>(content,
                new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            throw new Exception($"Login response invalid: {ex.Message}");
        }

        if (body?.StudUser == null)
            throw new Exception("Login failed: missing user data in response");

        return body.StudUser;
    }
}