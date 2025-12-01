using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Inventory
{
    public class BrickLinkInventoryDTO
    {
        [JsonPropertyName("inventory_id")] public long InventoryId { get; set; }

        [JsonPropertyName("item")] public BrickLinkItemDTO Item { get; set; }

        [JsonPropertyName("color_id")] public int ColorId { get; set; }

        [JsonPropertyName("quantity")] public int Quantity { get; set; }

        [JsonPropertyName("new_or_used")]
        public string NewOrUsed { get; set; } // "N" or "U"

        // Use string for currency fields to preserve API format ("X.XXXX")
        [JsonPropertyName("unit_price")] public string UnitPrice { get; set; }

        [JsonPropertyName("bind_id")] public int BindId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("remarks")] public string Remarks { get; set; }

        [JsonPropertyName("bulk")] public int Bulk { get; set; }

        [JsonPropertyName("is_retain")] public bool IsRetain { get; set; }

        [JsonPropertyName("is_stock_room")]
        public bool IsStockRoom { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("sale_rate")] public int SaleRate { get; set; }

        // Tiered Pricing Fields
        [JsonPropertyName("my_cost")] public string MyCost { get; set; }

        [JsonPropertyName("tier_quantity1")]
        public int TierQuantity1 { get; set; }

        [JsonPropertyName("tier_price1")] public string TierPrice1 { get; set; }

        [JsonPropertyName("tier_quantity2")]
        public int TierQuantity2 { get; set; }

        [JsonPropertyName("tier_price2")] public string TierPrice2 { get; set; }

        [JsonPropertyName("tier_quantity3")]
        public int TierQuantity3 { get; set; }

        [JsonPropertyName("tier_price3")] public string TierPrice3 { get; set; }
    }
}