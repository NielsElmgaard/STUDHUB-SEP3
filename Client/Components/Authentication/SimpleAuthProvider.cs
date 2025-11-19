using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Users;

namespace Client.Components.Authentication;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        if (string.IsNullOrEmpty(userAsJson))
            return new AuthenticationState(new ClaimsPrincipal());

        StudUserDTO? userDto;
        try
        {
            userDto = JsonSerializer.Deserialize<StudUserDTO>(userAsJson);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        if (userDto == null)
            return new AuthenticationState(new ClaimsPrincipal());

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "apiauth");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    public async Task LoginASync(string email, string password)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login",
            new LoginRequestDTO { Email = email, Password = password });

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception(content);

        LoginResponseDTO? loginResponse;
        try
        {
            loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            throw new Exception("Login response invalid");
        }

        if (loginResponse?.StudUser == null)
            throw new Exception("Login failed: missing user data");

        await RefreshUser(loginResponse.StudUser);
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    public async Task RefreshUser(StudUserDTO? userDto)
    {
        if (userDto == null) throw new ArgumentNullException(nameof(userDto));

        string serializedData = JsonSerializer.Serialize(userDto);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serializedData);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "apiauth");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
