using System.ComponentModel.DataAnnotations;

namespace StudHub.SharedDTO.StoreCredentials;

public class BrickOwlCredentialsRequestDTO
{[Required(ErrorMessage = "The Brick Owl API Key field is required.")]
    public string BrickOwlApiKey { get; set; } = "";
    public int StudUserId { get; set; }

}