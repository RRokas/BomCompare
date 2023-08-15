using Core.Entitites;

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
                var comparedBomLine = sourceBomLine.CreateCopyWithoutDesignators();
                foreach (var designator in sourceBomLine.Designators)
                    comparedBomLine.Designators.Add(new Designator(designator.Name, ComparisonStatus.Removed));
            
                comparedBom.Add(comparedBomLine);
            }
            else
            {
                comparedBom.Add(
                    CompareBomLine(sourceBomLine, targetBomLine)
                );
            }
            
        }
        
        var newPartNumbersInTarget = target
            .Select(t => t.PartNumber)
            .Except(source.Select(s => s.PartNumber))
            .ToList();
        foreach (var bomLine in target.Where(x => newPartNumbersInTarget.Contains(x.PartNumber)))
        {
            var comparedBomLine = bomLine.CreateCopyWithoutDesignators();
            foreach (var designator in bomLine.Designators)
                comparedBomLine.Designators.Add(new Designator(designator.Name, ComparisonStatus.Added));
            
            comparedBom.Add(comparedBomLine);
        }

        return comparedBom;
    }

    public BomLine CompareBomLine(BomLine source, BomLine target)
    {
        if (source.PartNumber != target.PartNumber)
            throw new ArgumentException("Part numbers must match to compare BOM lines");

        var comparedBomLine = source.CreateCopyWithoutDesignators();

        foreach (var sourceDesignator in source.Designators)
        {
            var targetDesignator = target.Designators.FirstOrDefault(x => x.Name == sourceDesignator.Name);
            comparedBomLine.Designators.Add(targetDesignator is null
                ? new Designator(sourceDesignator.Name, ComparisonStatus.Removed)
                : new Designator(sourceDesignator.Name, ComparisonStatus.Unchanged));
        }
        
        foreach (var targetDesignator in target.Designators)
        {
            var sourceDesignator = source.Designators.FirstOrDefault(x => x.Name == targetDesignator.Name);
            if (sourceDesignator is null)
            {
                comparedBomLine.Designators.Add(new Designator(targetDesignator.Name, ComparisonStatus.Added));
            }
        }
        
        return comparedBomLine;
    }
    
    
}