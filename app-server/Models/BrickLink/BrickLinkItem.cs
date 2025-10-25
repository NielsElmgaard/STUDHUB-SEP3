using System.Text.Json.Serialization;

namespace StudHub.SharedDTO;

public class BrickLinkItem
{
    [JsonPropertyName("no")]
    public string Number { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }
}