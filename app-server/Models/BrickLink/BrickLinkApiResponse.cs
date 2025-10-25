using System.Text.Json.Serialization;

namespace StudHub.SharedDTO;

public class BrickLinkApiResponse<T>
{
    [JsonPropertyName("meta")]
    public BrickLinkMeta Meta { get; set; }

    [JsonPropertyName("data")]
    public T Data { get; set; }
}