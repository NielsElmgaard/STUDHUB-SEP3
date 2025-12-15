namespace Studhub.AppServer.Services.Api_Auth;

public interface IApiAuthService
{
    Task<List<T>> GetBrickLinkResponse<T>(int studUserId, string url, Dictionary<string, string>? queryParams = null);
    Task<List<T>> GetBrickOwlResponse<T>(int studUserId, string url, Dictionary<string, string>? queryParams = null);
    Task<T> PostBrickOwlResponse<T>(int studUserId, string url, Dictionary<string, string> formData);
}