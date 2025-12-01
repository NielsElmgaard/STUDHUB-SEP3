using System.Security.Claims;
using System.Text.Json;
using Client.Services.Auth_Login;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Users;

namespace Client.Components.Authentication;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly ILoginClientService loginClientService;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson =
                await jsRuntime.InvokeAsync<string>("sessionStorage.getItem",
                    "currentUser");
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

    public async Task LoginAsync(string email, string password)
    {
        var request = new LoginRequestDTO
            { Email = email.Trim(), Password = password.Trim() };

        StudUserDTO userDto = await loginClientService.LoginUserAsync(request);

        await RefreshUser(userDto);
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser",
            "");
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    public async Task RefreshUser(StudUserDTO? userDto)
    {
        if (userDto == null) throw new ArgumentNullException(nameof(userDto));

        string serializedData = JsonSerializer.Serialize(userDto);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser",
            serializedData);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "apiauth");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(principal)));
    }
}