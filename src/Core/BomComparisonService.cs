using System.Data;
using Core.Entitites;

namespace Core;

public interface IBomComparisonService
{
    public List<ComparedBomLine> CompareBom(List<BomLine> source, List<BomLine> target);
    public ComparedBomLine CompareBomLine(BomLine source, BomLine target);
}

public class BomComparisonService : IBomComparisonService
{
    public List<ComparedBomLine> CompareBom(List<BomLine> source, List<BomLine> target)
    {
        var comparedBom = new List<ComparedBomLine>();
        foreach (var sourceBomLine in source)
        {
            var targetBomLine = target.FirstOrDefault(x => x.PartNumber == sourceBomLine.PartNumber);
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
        
        var newPartNumbersInTarget = target
            .Select(t => t.PartNumber)
            .Except(source.Select(s => s.PartNumber))
            .ToList();
        foreach (var bomLine in target.Where(x => newPartNumbersInTarget.Contains(x.PartNumber)))
        {
            var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(bomLine);
            foreach (var designator in bomLine.Designators)
                comparedBomLine.Designators.Add(new Designator(designator.Name, DesignatorComparisonStatus.Added));
            comparedBomLine.ComparisonStatus = BomLineComparisonStatus.Added;
            comparedBom.Add(comparedBomLine);
        }

        return comparedBom;
    }

    public ComparedBomLine CompareBomLine(BomLine source, BomLine target)
    {
        if (source.PartNumber != target.PartNumber)
            throw new ArgumentException("Part numbers must match to compare BOM lines");

        var comparedBomLine = ComparedBomLine.FromBomLineWithoutDesignators(source);

        foreach (var sourceDesignator in source.Designators)
        {
            var targetDesignator = target.Designators.FirstOrDefault(x => x.Name == sourceDesignator.Name);
            comparedBomLine.Designators.Add(targetDesignator is null
                ? new Designator(sourceDesignator.Name, DesignatorComparisonStatus.Removed)
                : new Designator(sourceDesignator.Name, DesignatorComparisonStatus.Unchanged));
        }
        
        foreach (var targetDesignator in target.Designators)
        {
            var sourceDesignator = source.Designators.FirstOrDefault(x => x.Name == targetDesignator.Name);
            if (sourceDesignator is null)
            {
                comparedBomLine.Designators.Add(new Designator(targetDesignator.Name, DesignatorComparisonStatus.Added));
            }
        }
        
        comparedBomLine.ComparisonStatus = DetermineBomLineComparisonStatus(comparedBomLine.Designators);
        
        return comparedBomLine;
    }

    private BomLineComparisonStatus DetermineBomLineComparisonStatus(List<Designator> designators)
    {
        var distinctStatusesExcludingUnchanged = designators
            .Select(x => x.DesignatorComparisonStatus)
            .Distinct()
            .Where(x => x != DesignatorComparisonStatus.Unchanged)
            .ToList();

        if (distinctStatusesExcludingUnchanged.Count == 0)
            return BomLineComparisonStatus.Unchanged;
        if (distinctStatusesExcludingUnchanged.Count > 1)
            return BomLineComparisonStatus.Modified;
        if (distinctStatusesExcludingUnchanged[0] == DesignatorComparisonStatus.Added)
            return BomLineComparisonStatus.Added;
        if (distinctStatusesExcludingUnchanged[0] == DesignatorComparisonStatus.Removed)
            return BomLineComparisonStatus.Removed;
        
        throw new ConstraintException("Unhandled comparison status");
    }
}