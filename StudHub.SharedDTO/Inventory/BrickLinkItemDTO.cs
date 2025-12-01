using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Inventory;

public class BrickLinkItemDTO
{
    [JsonPropertyName("no")]
    public string No { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("categoryID")]
    public int CategoryID { get; set; }
}