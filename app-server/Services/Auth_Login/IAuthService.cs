using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;

namespace Studhub.AppServer.Services.Auth_Login;

public interface IAuthService
{
    Task<string?> ValidateUserAsync(string email, string password);

    Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(string email,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);

    Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(string email);

    Task<BrickLinkConnectionTestDTO?>
        TestBrickLinkConnectionAsync(string email,
            string consumerKey, string consumerSecret, string tokenValue,
            string tokenSecret);

    Task<BrickOwlCredentialsResponseDTO?> SetBrickOwlCredentialsAsync(
        string email,
        string brickOwlApiKey);

    Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(string email);

    Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(string email,
        string brickOwlApiKey);
}