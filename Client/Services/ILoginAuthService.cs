using StudHub.SharedDTO;

namespace Client.Services;

public interface ILoginAuthService
{
    Task<string> LoginUserAsync(LoginRequestDTO request);
}