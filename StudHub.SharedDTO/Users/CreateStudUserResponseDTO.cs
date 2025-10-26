namespace StudHub.SharedDTO.Users;

public class CreateStudUserResponseDTO
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}