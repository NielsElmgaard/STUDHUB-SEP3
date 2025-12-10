using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Inventory;

public class BrickOwlUpdateLotDto
{
    [JsonPropertyName("status")] public string Status { get; set; } // e.g. Success
}