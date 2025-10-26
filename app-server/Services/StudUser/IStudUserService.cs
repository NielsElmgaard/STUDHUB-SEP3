using StudHub.SharedDTO.Users;

namespace Studhub.AppServer.Services.StudUser;

public interface IStudUserService
{
    Task<CreateStudUserResponseDTO> CreateStudUser(CreateStudUserRequestDTO userRequest);
}