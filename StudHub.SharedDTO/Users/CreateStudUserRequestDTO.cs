using System.ComponentModel.DataAnnotations;

namespace StudHub.SharedDTO.Users;

public class CreateStudUserRequestDTO
{
    [Required(ErrorMessage = "Username is required and cannot be empty.")]

    public required string Username { get; set; }
    
    [Required(ErrorMessage = "Email is required and cannot be empty.")]

    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required and cannot be empty.")]
    public required string Password { get; set; }
}