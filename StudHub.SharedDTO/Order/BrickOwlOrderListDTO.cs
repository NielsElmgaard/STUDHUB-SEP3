using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order;

public class BrickOwlOrderListDto
{
    [JsonPropertyName("order_id")] public string OrderId { get; set; } = default!;

    [JsonPropertyName("order_date")] public string OrderDate { get; set; } = default!; // Unix timestamp (seconds)

    [JsonPropertyName("total_quantity")] public string TotalQuantity { get; set; } = default!;

    [JsonPropertyName("total_lots")] public string TotalLots { get; set; } = default!;

    [JsonPropertyName("base_order_total")] public string BaseOrderTotal { get; set; } = default!;

    [JsonPropertyName("status")] public string Status { get; set; } = default!;

    [JsonPropertyName("status_id")] public string StatusId { get; set; } = default!;
}