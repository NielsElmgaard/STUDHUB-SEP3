using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;
using StudHub.SharedDTO.Users;

namespace Studhub.AppServer.Services.Auth_Login;

public interface IAuthService
{
    Task<StudUserDTO?> ValidateUserAsync(string email, string password);

    Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(long studUserId,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);

    Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(long studUserId);

    Task<BrickLinkConnectionTestDTO?>
        TestBrickLinkConnectionAsync(
            string consumerKey, string consumerSecret, string tokenValue,
            string tokenSecret);

    Task<BrickOwlCredentialsDTO?> SetBrickOwlCredentialsAsync(
        long studUserId,
        string brickOwlApiKey);

    Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(long studUserId);

    Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(
        string brickOwlApiKey);
    
    Task<BrickLinkCredentialsDTO?> ClearBrickLinkCredentialsAsync(long studUserId,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);
    
    Task<BrickOwlCredentialsDTO?> ClearBrickOwlCredentialsAsync(
        long studUserId,
        string brickOwlApiKey);

    Task<bool> IsBrickLinkConnectedAsync(long studUserId);

    Task<bool> IsBrickOwlConnectedAsync(long studUserId);
}