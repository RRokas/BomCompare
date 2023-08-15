namespace Core.Tests;

public class BomLineComparisonTests
{
    [Fact]
    public void AddedDesignator()
    {
        var comparer = new BomComparisonService();
        var source = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
            }
        };
        
        var target = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
                new Designator { Name = "R4" },
            }
        };
        
        var result = comparer.CompareBomLine(source, target);
        Assert.Equal(4, result.Designators.Count);
        Assert.Equal(ComparisonStatus.Added, result.Designators[3].ComparisonStatus);
    }
    
    [Fact]
    public void RemovedDesignator()
    {
        var comparer = new BomComparisonService();
        var source = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
            }
        };
        
        var target = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
            }
        };
        
        var result = comparer.CompareBomLine(source, target);
        Assert.Equal(3, result.Designators.Count);
        Assert.Equal(ComparisonStatus.Removed, result.Designators[2].ComparisonStatus);
    }
    
    [Fact]
    public void UnchangedDesignator()
    {
        var comparer = new BomComparisonService();
        var source = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
            }
        };
        
        var target = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
            }
        };
        
        var result = comparer.CompareBomLine(source, target);
        Assert.Equal(3, result.Designators.Count);
        Assert.All(result.Designators,
            x => Assert.Equal(ComparisonStatus.Unchanged, x.ComparisonStatus));
    }
    
    [Fact]
    public void MixedResults()
    {
        var comparer = new BomComparisonService();
        var source = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R2" },
                new Designator { Name = "R3" },
            }
        };
        
        var target = new BomLine
        {
            Quantity = 1,
            PartNumber = "123",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R3" },
                new Designator { Name = "R4" },
            }
        };
        
        var result = comparer.CompareBomLine(source, target);
        Assert.Equal(4, result.Designators.Count);
        Assert.Equal(ComparisonStatus.Unchanged, result.Designators[0].ComparisonStatus);
        Assert.Equal(ComparisonStatus.Removed, result.Designators[1].ComparisonStatus);
        Assert.Equal(ComparisonStatus.Unchanged, result.Designators[2].ComparisonStatus);
        Assert.Equal(ComparisonStatus.Added, result.Designators[3].ComparisonStatus);
    }
}