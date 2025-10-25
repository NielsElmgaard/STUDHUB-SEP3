using StudHub.SharedDTO;

namespace Client.Services;

public class LoginAuthService : ILoginAuthService
{
    private readonly HttpClient _httpClient;

    public LoginAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> LoginUserAsync(LoginRequestDTO request)
    {
        HttpResponseMessage httpResponse =
            await _httpClient.PostAsJsonAsync("auth/login", request);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {error}");
        }
        
        var body = await httpResponse.Content
            .ReadFromJsonAsync<LoginResponseDTO>();

        if (body == null)
        {
            throw new Exception("Empty response");
        }

        if (!string.IsNullOrEmpty(body.ErrorMessage))
            throw new Exception($"Login failed: {body.ErrorMessage}");
        
        // success
        return body.Username;
    }
}