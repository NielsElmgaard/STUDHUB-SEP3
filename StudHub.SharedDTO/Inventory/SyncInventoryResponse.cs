namespace StudHub.SharedDTO.Inventory;

public class SyncInventoryResponse
{
    public List<string?> Success { get; set; } = new();
    public List<string?> Failed { get; set; } = new();
}
