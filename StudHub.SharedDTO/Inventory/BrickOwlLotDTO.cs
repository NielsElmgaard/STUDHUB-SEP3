using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json; // Needed for the JsonElement handling

namespace StudHub.SharedDTO.Inventory;

public class BrickOwlLotDTO
{
    // --- Condition and Quantity Fields (MUST be string) ---

    [JsonPropertyName("con")]
    public string? Condition { get; set; } // e.g., "used"

    [JsonPropertyName("full_con")]
    public string? FullCondition { get; set; } // e.g., "usedc"

    // Quantity: Sent as a quoted string (e.g., "1").
    [JsonPropertyName("qty")]
    public string? Quantity { get; set; } 

    // --- ID Fields ---
    
    // Inventory Lot ID: Sent as a quoted string (e.g., "54803081").
    [JsonPropertyName("lot_id")]
    public string? LotId { get; set; } 

    // Item/Color/Condition Catalog ID: Sent as a quoted string.
    [JsonPropertyName("owl_id")]
    public string? OwlId { get; set; } 

    // Granular Catalog ID (Item/Color): Sent as a quoted string.
    [JsonPropertyName("boid")]
    public string? BOID { get; set; }

    // Item Type: e.g., "Part", "Minifigure", "Set".
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    // --- Price and Cost Fields (MUST be string) ---

    // Price: Sent as a quoted string (e.g., "10.751").
    [JsonPropertyName("price")]
    public string? Price { get; set; }

    // Base Price: Sent as a quoted string.
    [JsonPropertyName("base_price")]
    public string? BasePrice { get; set; } 

    // Final Price (after sale/discount): Sent as a quoted string.
    [JsonPropertyName("final_price")]
    public string? FinalPrice { get; set; } 

    // Sale Percentage: Sent as a quoted string.
    [JsonPropertyName("sale_percent")]
    public string? SalePercent { get; set; }

    // My Cost: Sent as a quoted string (may be null/missing).
    [JsonPropertyName("my_cost")]
    public string? MyCost { get; set; } 

    // --- Other Fields (MUST be string/complex) ---

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("public_note")]
    public string? PublicNote { get; set; }

    [JsonPropertyName("personal_note")]
    public string? PersonalNote { get; set; }

    // Bulk Quantity (min quantity for bulk price): Sent as a quoted string.
    [JsonPropertyName("bulk_qty")]
    public string? BulkQty { get; set; } 

    // For Sale status (0 or 1): Sent as a quoted string.
    [JsonPropertyName("for_sale")]
    public string? ForSale { get; set; } 

    // Weight: Sent as a quoted string.
    [JsonPropertyName("lot_weight")]
    public string? LotWeight { get; set; }

    [JsonPropertyName("reserve_uid")]
    public string? ReserveUid { get; set; }
    
    // IDs is an array of objects for external catalog IDs (e.g., BrickLink, LEGO).
    [JsonPropertyName("ids")]
    public List<IdDetailDTO>? Ids { get; set; }

    // Tier Price is an array of objects for bulk pricing levels.
    [JsonPropertyName("tier_price")]
    public List<TierPriceDTO>? TierPrice { get; set; }
    
    // --- Handling Inconsistent External Lot IDs (Recommended Robust Method) ---
    // Use JsonElement to handle cases where external_lot_ids might be null, [], or {}
    [JsonPropertyName("external_lot_ids")]
    public JsonElement? ExternalLotIdsRaw { get; set; }

    // Add a read-only property for easy access, handling the JSON structure inconsistency
    [JsonIgnore]
    public Dictionary<string, string>? ExternalLotIds
    {
        get
        {
            if (ExternalLotIdsRaw == null || ExternalLotIdsRaw.Value.ValueKind != JsonValueKind.Object)
            {
                return null;
            }
            // Safely attempt to deserialize the JSON object into a dictionary
            return ExternalLotIdsRaw.Value.Deserialize<Dictionary<string, string>>();
        }
    }

    // --- Nested DTOs ---

    public class IdDetailDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; } // ID is usually a string

        [JsonPropertyName("type")]
        public string? Type { get; set; } // e.g., "bl_item" (BrickLink Item)
    }

    public class TierPriceDTO
    {
        // Define fields based on the actual Tier Price structure (e.g., min_qty, price)
        [JsonPropertyName("min_qty")]
        public string? MinQuantity { get; set; }

        [JsonPropertyName("price")]
        public string? TierPriceValue { get; set; }
    }
}