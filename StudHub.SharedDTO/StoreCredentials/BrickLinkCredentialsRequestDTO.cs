using System.ComponentModel.DataAnnotations;

namespace StudHub.SharedDTO.StoreCredentials;

public class BrickLinkCredentialsRequestDTO
{
    [Required(ErrorMessage = "The Consumer Key field is required.")]
    public string ConsumerKey { get; set; } = "";

    [Required(ErrorMessage = "The Consumer Secret field is required.")]
    public string ConsumerSecret { get; set; } = "";

    [Required(ErrorMessage = "The Token Value field is required.")]
    public string TokenValue { get; set; } = "";

    [Required(ErrorMessage = "The Token Secret field is required.")]
    public string TokenSecret { get; set; } = "";

    public long StudUserId { get; set; }
}