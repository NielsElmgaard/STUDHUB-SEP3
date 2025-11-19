using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;

namespace Studhub.AppServer.Services.Auth_Login;

public interface IAuthService
{
    Task<string?> ValidateUserAsync(string email, string password);

    Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(long studUserId,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);

    Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(long studUserId);

    Task<BrickLinkConnectionTestDTO?>
        TestBrickLinkConnectionAsync(long studUserId,
            string consumerKey, string consumerSecret, string tokenValue,
            string tokenSecret);

    Task<BrickOwlCredentialsResponseDTO?> SetBrickOwlCredentialsAsync(
        long studUserId,
        string brickOwlApiKey);

    Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(long studUserId);

    Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(long studUserId,
        string brickOwlApiKey);
}