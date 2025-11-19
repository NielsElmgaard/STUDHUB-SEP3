using StudHub.SharedDTO.Users;

namespace StudHub.SharedDTO;

public class LoginResponseDTO
{
    public string Username { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    public StudUserDTO StudUser { get; set; } = null!;
}