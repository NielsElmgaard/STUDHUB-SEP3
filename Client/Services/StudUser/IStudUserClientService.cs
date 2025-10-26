using StudHub.SharedDTO.Users;

namespace Client.Services;

public interface IStudUserClientService
{
    Task<CreateStudUserResponseDTO> CreateStudUser(CreateStudUserRequestDTO userRequest);
}