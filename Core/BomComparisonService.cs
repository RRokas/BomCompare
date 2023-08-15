namespace Core;

public interface IBomComparisonService
{
    public List<BomLine> CompareBom(List<BomLine> source, List<BomLine> target);
    public BomLine CompareBomLine(BomLine source, BomLine target);
}

public class BomComparisonService : IBomComparisonService
{
    public List<BomLine> CompareBom(List<BomLine> source, List<BomLine> target)
    {
        var comparedBom = new List<BomLine>();
        foreach (var sourceBomLine in source)
        {
            var targetBomLine = target.FirstOrDefault(x => x.PartNumber == sourceBomLine.PartNumber);
            if(targetBomLine is null)
            {
                comparedBom.Add(
                    new BomLine
                    {
                        Quantity = sourceBomLine.Quantity,
                        PartNumber = sourceBomLine.PartNumber,
                        Designators = sourceBomLine.Designators.Select(x => new Designator { Name = x.Name, ComparisonStatus = ComparisonStatus.Removed }).ToList(),
                        Value = sourceBomLine.Value,
                        SMD = sourceBomLine.SMD,
                        Description = sourceBomLine.Description,
                        Manufacturer = sourceBomLine.Manufacturer,
                        ManufacturerPartNumber = sourceBomLine.ManufacturerPartNumber,
                        Distributor = sourceBomLine.Distributor,
                        DistributorPartNumber = sourceBomLine.DistributorPartNumber,
                    }
                );
            }
            else
            {
                comparedBom.Add(
                    CompareBomLine(sourceBomLine, targetBomLine)
                );
            }
            
        }

        return comparedBom;
    }

    public BomLine CompareBomLine(BomLine source, BomLine target)
    {
        if (source.PartNumber != target.PartNumber)
            throw new ArgumentException("Part numbers must match to compare BOM lines");
        
        var comparedBomLine = new BomLine();

        foreach (var sourceDesignator in source.Designators)
        {
            var targetDesignator = target.Designators.FirstOrDefault(x => x.Name == sourceDesignator.Name);
            comparedBomLine.Designators.Add(targetDesignator is null
                ? new Designator { Name = sourceDesignator.Name, ComparisonStatus = ComparisonStatus.Removed }
                : new Designator { Name = sourceDesignator.Name, ComparisonStatus = ComparisonStatus.Unchanged });
        }
        
        foreach (var targetDesignator in target.Designators)
        {
            var sourceDesignator = source.Designators.FirstOrDefault(x => x.Name == targetDesignator.Name);
            if (sourceDesignator is null)
            {
                comparedBomLine.Designators.Add(new Designator { Name = targetDesignator.Name, ComparisonStatus = ComparisonStatus.Added });
            }
        }
        
        return comparedBomLine;
    }
}