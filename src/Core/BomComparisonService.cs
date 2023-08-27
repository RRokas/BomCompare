using System.Data;
using Core.Entitites;
using Core.Enums;

namespace Core;

public interface IBomComparisonService
{
    public ComparedBom CompareBom(Bom source, Bom target);
    public ComparedBomLine CompareBomLine(BomLine source, BomLine target);
}

public class BomComparisonService : IBomComparisonService
{
    public ComparedBom CompareBom(Bom source, Bom target)
    {
        var comparedBom = new List<ComparedBomLine>();
        foreach (var sourceBomLine in source.BomLines)
        {
            var targetBomLine = target.BomLines.Find(x => x.PartNumber == sourceBomLine.PartNumber);
            if(targetBomLine is null)
            {
                var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(sourceBomLine);
                comparedBomLine.ComparisonStatus = BomLineComparisonStatus.Removed;
                foreach (var designator in sourceBomLine.Designators)
                    comparedBomLine.Designators.Add(new Designator(designator.Name, DesignatorComparisonStatus.Removed));
            
                comparedBom.Add(comparedBomLine);
            }
            else
            {
                comparedBom.Add(
                    CompareBomLine(sourceBomLine, targetBomLine)
                );
            }
            
        }
        
        var newPartNumbersInTarget = target.BomLines
            .Select(t => t.PartNumber)
            .Except(source.BomLines.Select(s => s.PartNumber))
            .ToList();
        foreach (var bomLine in target.BomLines.Where(x => newPartNumbersInTarget.Contains(x.PartNumber)))
        {
            var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(bomLine);
            foreach (var designator in bomLine.Designators)
                comparedBomLine.Designators.Add(new Designator(designator.Name, DesignatorComparisonStatus.Added));
            comparedBomLine.ComparisonStatus = BomLineComparisonStatus.Added;
            comparedBom.Add(comparedBomLine);
        }

        return new ComparedBom
        {
            SourceBom = source,
            TargetBom = target,
            ComparedBomLines = comparedBom
        };
    }

    public ComparedBomLine CompareBomLine(BomLine source, BomLine target)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));
        _ = target ?? throw new ArgumentNullException(nameof(target));
        if (source.PartNumber != target.PartNumber)
            throw new ArgumentException("Part numbers must match to compare BOM lines");

        var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(source);

        foreach (var sourceDesignatorName in source.Designators.Select(x=>x.Name))
        {
            var targetDesignator = target.Designators.Find(x => x.Name == sourceDesignatorName);
            comparedBomLine.Designators.Add(targetDesignator is null
                ? new Designator(sourceDesignatorName, DesignatorComparisonStatus.Removed)
                : new Designator(sourceDesignatorName, DesignatorComparisonStatus.Unchanged));
        }
        
        foreach (var targetDesignatorName in target.Designators.Select(x=>x.Name))
        {
            var sourceDesignator = source.Designators.Find(x => x.Name == targetDesignatorName);
            if (sourceDesignator is null)
            {
                comparedBomLine.Designators.Add(new Designator(targetDesignatorName, DesignatorComparisonStatus.Added));
            }
        }
        
        comparedBomLine.ComparisonStatus = DetermineBomLineComparisonStatus(comparedBomLine.Designators);
        
        return comparedBomLine;
    }

    private BomLineComparisonStatus DetermineBomLineComparisonStatus(List<Designator> designators)
    {
        var distinctStatuses = designators
            .Select(x => x.DesignatorComparisonStatus)
            .Distinct()
            .ToList();
        
        if(distinctStatuses.Count == 1 && distinctStatuses[0] == DesignatorComparisonStatus.Unchanged)
            return BomLineComparisonStatus.Unchanged;
        if(distinctStatuses.Count == 1 && distinctStatuses[0] == DesignatorComparisonStatus.Added)
            return BomLineComparisonStatus.Added;
        if(distinctStatuses.Count == 1 && distinctStatuses[0] == DesignatorComparisonStatus.Removed)
            return BomLineComparisonStatus.Removed;
        if(distinctStatuses.Count > 1)
            return BomLineComparisonStatus.Modified;
        
        throw new ConstraintException("Unhandled comparison status");
    }
}