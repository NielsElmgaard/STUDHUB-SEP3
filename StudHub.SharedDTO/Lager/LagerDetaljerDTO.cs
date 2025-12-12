namespace StudHub.SharedDTO.Lager;

public class LagerDetaljerDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public int Quantity { get; set; }
    
    public string? Color { get; set; }
    
    public string? PartId { get; set; }
    
    public string? Remarks { get; set; }
    
    public decimal? PricePerUnit { get; set; }
}