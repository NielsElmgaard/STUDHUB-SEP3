using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Inventory;

public class BrickOwlCreateLotDTO
{
    [JsonPropertyName("status")] public string Status { get; set; } // e.g. Success
    [JsonPropertyName("log_id")] public string LotId { get; set; } // e.g. 207596407
}