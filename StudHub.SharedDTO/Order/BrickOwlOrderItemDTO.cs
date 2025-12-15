using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order;

public class BrickOwlOrderItemDto
{
    // FAKE RESPONSE DTO
    [JsonPropertyName("boid")] public string BOID { get; set; }
    [JsonPropertyName("quantity")] public string Quantity { get; set; }
}