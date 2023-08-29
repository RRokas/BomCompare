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
        var distinctPartNumbers = source.BomLines
            .Select(x => x.PartNumber)
            .Union(target.BomLines.Select(x => x.PartNumber))
            .Distinct()
            .ToList();
        
        foreach (var partNumber in distinctPartNumbers)
        {
            var sourceBomLine = source.BomLines.Find(x => x.PartNumber == partNumber);
            var targetBomLine = target.BomLines.Find(x => x.PartNumber == partNumber);
            if (sourceBomLine is not null && targetBomLine is not null)
            {
                var comparedBomLine = CompareBomLine(sourceBomLine, targetBomLine);
                comparedBom.Add(comparedBomLine);
            }
            else if (sourceBomLine is not null && targetBomLine is null)
            {
                comparedBom.Add(CreateRemovedBomLine(sourceBomLine));
            }
            else if (targetBomLine is not null && sourceBomLine is null)
            {
                comparedBom.Add(CreateAddedBomLine(targetBomLine));
            }
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
        var distinctDesignators = DistinctDesignatorsWithSourceOrdering(source.Designators, target.Designators);
        
        foreach (var designatorName in distinctDesignators)
        {
            var sourceDesignatorName = source.Designators.Find(x => x.Name == designatorName);
            var targetDesignatorName = target.Designators.Find(x => x.Name == designatorName);
            if(sourceDesignatorName is not null && targetDesignatorName is not null)
                comparedBomLine.Designators.Add(new Designator(designatorName, DesignatorComparisonStatus.Unchanged));
            else if (sourceDesignatorName is not null && targetDesignatorName is null)
                comparedBomLine.Designators.Add(new Designator(designatorName, DesignatorComparisonStatus.Removed));
            else if (sourceDesignatorName is null && targetDesignatorName is not null)
                comparedBomLine.Designators.Add(new Designator(designatorName, DesignatorComparisonStatus.Added));
        }
        
        comparedBomLine.ComparisonStatus = DetermineBomLineComparisonStatus(comparedBomLine.Designators);
        
        return comparedBomLine;
    }
    
    private ComparedBomLine CreateAddedBomLine(BomLine targetBomLine)
    {
        var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(targetBomLine);
        comparedBomLine.ComparisonStatus = BomLineComparisonStatus.Added;
        AddDesignators(comparedBomLine.Designators, targetBomLine.Designators, DesignatorComparisonStatus.Added);
        return comparedBomLine;
    }
    
    private ComparedBomLine CreateRemovedBomLine(BomLine sourceBomLine)
    {
        var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(sourceBomLine);
        comparedBomLine.ComparisonStatus = BomLineComparisonStatus.Removed;
        AddDesignators(comparedBomLine.Designators, sourceBomLine.Designators, DesignatorComparisonStatus.Removed);
        return comparedBomLine;
    }
    
    private static void AddDesignators(List<Designator> comparedDesignators, List<Designator> designators, DesignatorComparisonStatus status)
    {
        foreach (var designator in designators)
            comparedDesignators.Add(new Designator(designator.Name, status));
    }

    private List<string> DistinctDesignatorsWithSourceOrdering(List<Designator> source, List<Designator> target)
    {
        var sourceNames = source.Select(x => x.Name).Distinct().ToList();
        var targetNames = target.Select(x => x.Name).Distinct().ToList();
        
        var distinctDesignatorsWithSourceOrdering = sourceNames;

        for (int i = 0; i < targetNames.Count; i++)
        {
            if(distinctDesignatorsWithSourceOrdering.Contains(targetNames[i]))
                continue;
            
            distinctDesignatorsWithSourceOrdering.Insert(i, targetNames[i]);
        }

        return distinctDesignatorsWithSourceOrdering;
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