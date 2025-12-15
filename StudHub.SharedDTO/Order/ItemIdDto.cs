using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order;

public class ItemIdDto
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;

    [JsonPropertyName("type")] public string Type { get; set; } = default!;
}