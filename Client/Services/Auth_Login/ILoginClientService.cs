using StudHub.SharedDTO;
using StudHub.SharedDTO.Users;

namespace Client.Services.Auth_Login;

public interface ILoginClientService
{
    Task<StudUserDTO> LoginUserAsync(LoginRequestDTO request);
}