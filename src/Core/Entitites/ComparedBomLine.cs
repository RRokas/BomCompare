using Core.Attributes;
using Core.Enums;

namespace Core.Entitites;

public class ComparedBomLine : BomLine
{
    [ExcelColumnName("Comparison Status")]
    public BomLineComparisonStatus ComparisonStatus { get; set; }
    
    public static ComparedBomLine FromBomLineWithoutDesignators(BomLine sourceBomLine)
    {
        return new ComparedBomLine
        {
            Quantity = sourceBomLine.Quantity,
            PartNumber = sourceBomLine.PartNumber,
            Designators = new List<Designator>(),
            Value = sourceBomLine.Value,
            SMD = sourceBomLine.SMD,
            Description = sourceBomLine.Description,
            Manufacturer = sourceBomLine.Manufacturer,
            ManufacturerPartNumber = sourceBomLine.ManufacturerPartNumber,
            Distributor = sourceBomLine.Distributor,
            DistributorPartNumber = sourceBomLine.DistributorPartNumber,
        };
    }
}