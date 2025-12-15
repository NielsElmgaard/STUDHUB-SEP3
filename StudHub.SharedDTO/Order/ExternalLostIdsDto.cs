using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order;

public class ExternalLotIdsDto
{
    [JsonPropertyName("other")] public string? Other { get; set; }
}