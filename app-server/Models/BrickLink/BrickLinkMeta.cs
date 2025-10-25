using System.Text.Json.Serialization;

namespace StudHub.SharedDTO;

public class BrickLinkMeta
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}