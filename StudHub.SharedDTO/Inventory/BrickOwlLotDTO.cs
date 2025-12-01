using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Inventory
{
    public class BrickOwlLotDTO
    {
        // Brick Owl Lot ID. Nullable, only exists after a lot is created on BO.
        // Used to call inventory/update or inventory/delete.
        [JsonPropertyName("lot_id")]
        public long? LotId { get; set; } 

        // CRUCIAL: Your ID (e.g., BrickLink InventoryId or StudHub Primary Key)
        // Used to identify the lot you want to update/delete. 
        [JsonPropertyName("external_id")]
        public long? ExternalId { get; set; } 

        // --- ITEM IDENTIFIERS (FOR CREATING NEW LOTS) ---

        // Brick Owl Item ID. You must map BrickLink's (No + Type) to this. Required for 'create'.
        [JsonPropertyName("boid")]
        public string BOID { get; set; } 

        // Brick Owl Color ID. Required for 'create' of Parts.
        [JsonPropertyName("color_id")]
        public int? ColorId { get; set; } 
        
        // --- UPDATE/CREATE FIELDS ---

        // Quantity for 'create', or Absolute Quantity for 'update'. Must be > 0 for 'create'.
        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; } 

        // Price. Used in 'create' and 'update'. Use decimal to work with currency in C#.
        [JsonPropertyName("price")]
        public decimal? Price { get; set; } 

        // Condition ID (e.g., "new", "usedn"). Required for 'create'.
        // Must be mapped from BrickLink's "N" or "U".
        [JsonPropertyName("condition")]
        public string Condition { get; set; } 
        
        // My Cost
        [JsonPropertyName("my_cost")]
        public decimal? MyCost { get; set; }

        // Public Note (from BrickLink's 'description' or 'remarks')
        [JsonPropertyName("public_note")]
        public string PublicNote { get; set; }
        
        // Personal Note (from BrickLink's 'description' or 'remarks')
        [JsonPropertyName("personal_note")]
        public string PersonalNote { get; set; }

        // Bulk Quantity (from BrickLink's 'bulk')
        [JsonPropertyName("bulk_qty")]
        public int? BulkQty { get; set; }

        // Tier Price string, formatted as 'qty1:price1,qty2:price2,...'
        [JsonPropertyName("tier_price")]
        public string TierPrices { get; set; } 

        // For Sale status (0 or 1)
        [JsonPropertyName("for_sale")]
        public int? ForSale { get; set; } // 1 by default for inventory you want to sell

        // Use to change the external ID on an existing lot
        [JsonPropertyName("update_external_id_1")]
        public long? UpdateExternalId1 { get; set; } 

        // Only for Update: A positive or negative amount to adjust the existing quantity by
        [JsonPropertyName("relative_quantity")]
        public int? RelativeQuantity { get; set; } 
    }
}