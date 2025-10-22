namespace StudHub.SharedDTO;

public class SetDTO
{
    public string ItemNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public string
        Completeness { get; set; } =
        string.Empty; // C: Complete, B: Incomplete, S: Sealed
}