using System.Globalization;
using System.Text.Json.Serialization;

namespace StudHub.SharedDTO;

public class SetDTO
{
    public string ItemNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public string Condition { get; set; } = string.Empty; // N: New, U: Used

    public string PriceString { get; set; }

    [JsonIgnore]
    public decimal Price =>
        decimal.TryParse(PriceString, NumberStyles.Any,
            CultureInfo.InvariantCulture, out var price)
            ? price
            : 0m;
    
    [JsonIgnore]
    public string PriceFormatted => Price.ToString("F2", CultureInfo.InvariantCulture);

    public string
        Completeness { get; set; } =
        string.Empty; // C: Complete, B: Incomplete, S: Sealed
}