using System.ComponentModel.DataAnnotations;

namespace StudHub.SharedDTO.Users;

public class CreateStudUserRequestDTO
{
    [Required(ErrorMessage = "Username is required and cannot be empty.")]
    [StringLength(25, ErrorMessage = "Username may be at most 25 characters long.")]
    [RegularExpression("^[A-Za-z0-9._-]{1,25}$",
        ErrorMessage = "Username may only contain letters, digits, and . _ -")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Email is required and cannot be empty.")]
    [EmailAddress(ErrorMessage = "Email must be a valid format.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required and cannot be empty.")]
    [StringLength(32, MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 32 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$",
        ErrorMessage = "Password must contain lowercase, uppercase, a number, and a special character.")]
    public required string Password { get; set; }
}