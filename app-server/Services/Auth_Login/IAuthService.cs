using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;
using StudHub.SharedDTO.Users;

namespace Studhub.AppServer.Services.Auth_Login;

public interface IAuthService
{
    Task<StudUserDTO?> ValidateUserAsync(string email, string password);

    Task<BrickLinkCredentialsDTO?> SetBrickLinkCredentialsAsync(int studUserId,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);

    Task<BrickLinkCredentialsDTO?> GetBrickLinkCredentialsAsync(int studUserId);

    Task<BrickLinkConnectionTestDTO?>
        TestBrickLinkConnectionAsync(
            string consumerKey, string consumerSecret, string tokenValue,
            string tokenSecret);

    Task<BrickOwlCredentialsDTO?> SetBrickOwlCredentialsAsync(
        int studUserId,
        string brickOwlApiKey);

    Task<BrickOwlCredentialsDTO?> GetBrickOwlCredentialsAsync(int studUserId);

    Task<BrickOwlConnectionTestDTO?> TestBrickOwlConnectionAsync(
        string brickOwlApiKey);
    
    Task<BrickLinkCredentialsDTO?> ClearBrickLinkCredentialsAsync(int studUserId,
        string consumerKey, string consumerSecret, string tokenValue,
        string tokenSecret);
    
    Task<BrickOwlCredentialsDTO?> ClearBrickOwlCredentialsAsync(
        int studUserId,
        string brickOwlApiKey);

    Task<bool> IsBrickLinkConnectedAsync(int studUserId);

    Task<bool> IsBrickOwlConnectedAsync(int studUserId);
}