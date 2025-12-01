using StudHub.SharedDTO.Users;

namespace Client.Services.StudUser;

public interface IStudUserClientService
{
    Task<CreateStudUserResponseDTO> CreateStudUser(CreateStudUserRequestDTO userRequest);
}