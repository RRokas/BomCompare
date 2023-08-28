using Core.Attributes;

namespace Core.Entitites;

public class BomLine
{
    [ExcelColumnName("Quantity")]
    public int Quantity { get; set; }
    [ExcelColumnName("Part Number")]
    public string PartNumber { get; set; }
    [ExcelColumnName("Designator")]
    public List<Designator> Designators { get; set; } = new List<Designator>();
    [ExcelColumnName("Value")]
    public string Value { get; set; }
    [ExcelColumnName("SMD")]
    public string SMD { get; set; }
    [ExcelColumnName("Description")]
    public string Description { get; set; }
    [ExcelColumnName("Manufacturer")]
    public string Manufacturer { get; set; }
    [ExcelColumnName("Manufacturer Part Number")]
    public string ManufacturerPartNumber { get; set; }
    [ExcelColumnName("Distributor")]
    public string Distributor { get; set; }
    [ExcelColumnName("Distributor Part Number")]
    public string DistributorPartNumber { get; set; }
}