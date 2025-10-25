using System.Text.Json.Serialization;

namespace StudHub.SharedDTO;

public class BrickLinkInventoryItem
{
    [JsonPropertyName("inventory_id")]
    public int InventoryId { get; set; }

    [JsonPropertyName("item")]
    public BrickLinkItem ItemInfo { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("new_or_used")]
    public string Condition { get; set; }

    [JsonPropertyName("unit_price")]
    public string Price { get; set; }
    
    [JsonPropertyName("completeness")]
    public string Completeness { get; set; }
}