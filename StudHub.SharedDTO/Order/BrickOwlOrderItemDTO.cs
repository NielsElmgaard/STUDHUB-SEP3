using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order;

using System.Text.Json.Serialization;

public class BrickOwlOrderItemDto
{
    [JsonPropertyName("image_small")]
    public string ImageSmall { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("color_name")]
    public string ColorName { get; set; } = default!;

    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    [JsonPropertyName("boid")]
    public string Boid { get; set; } = default!;

    [JsonPropertyName("lot_id")]
    public long LotId { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = default!;

    [JsonPropertyName("full_con")]
    public string FullCondition { get; set; } = default!;

    [JsonPropertyName("ordered_quantity")]
    public int OrderedQuantity { get; set; }

    [JsonPropertyName("refunded_quantity")]
    public int RefundedQuantity { get; set; }

    [JsonPropertyName("personal_note")]
    public string? PersonalNote { get; set; }

    [JsonPropertyName("bl_lot_id")]
    public long? BrickLinkLotId { get; set; }

    [JsonPropertyName("external_lot_ids")]
    public ExternalLotIdsDto ExternalLotIds { get; set; } = default!;

    [JsonPropertyName("remaining_quantity")]
    public int RemainingQuantity { get; set; }

    [JsonPropertyName("weight")]
    public decimal Weight { get; set; }

    [JsonPropertyName("public_note")]
    public string? PublicNote { get; set; }

    [JsonPropertyName("order_item_id")]
    public long OrderItemId { get; set; }

    [JsonPropertyName("base_price")]
    public decimal BasePrice { get; set; }

    [JsonPropertyName("ids")]
    public List<ItemIdDto> Ids { get; set; } = new();
}
