using StudHub.SharedDTO;

namespace Client.Services;

public interface ILoginClientService
{
    Task<string> LoginUserAsync(LoginRequestDTO request);
}