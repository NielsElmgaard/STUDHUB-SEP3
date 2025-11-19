namespace StudHub.SharedDTO.StoreCredentials;

public class BrickLinkCredentialsRequestDTO
{
    public string ConsumerKey { get; set; } = "";
    public string ConsumerSecret { get; set; } = "";
    public string TokenValue { get; set; } = "";
    public string TokenSecret { get; set; } = "";
    public long StudUserId { get; set; }
}