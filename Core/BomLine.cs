namespace Core;

public class BomLine
{
    public int Quantity { get; set; }
    public string PartNumber { get; set; }
    public List<string> Designators { get; set; }
    public string Value { get; set; }
    public string SMD { get; set; }
    public string Description { get; set; }
    public string Manufacturer { get; set; }
    public string ManufacturerPartNumber { get; set; }
    public string Distributor { get; set; }
    public string DistributorPartNumber { get; set; }
}