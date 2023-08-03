namespace Core;

public class BomLine
{
    public string InternalPartId { get; set; }
    public string ManufacturerPartId { get; set; }
    public string ManufacturerName { get; set; }
    public string PartDescription { get; set; }
    public double Quantity { get; set; }
    public string Value { get; set; }
    public string Positions { get; set; }
}