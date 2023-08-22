using Core.Entitites;

namespace Core.Tests;

public class BomComparisonServiceTests
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
        Assert.Equal(DesignatorComparisonStatus.Added, result.Designators[3].DesignatorComparisonStatus);
        Assert.Equal(BomLineComparisonStatus.Modified, result.ComparisonStatus);
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
        Assert.Equal(DesignatorComparisonStatus.Removed, result.Designators[2].DesignatorComparisonStatus);
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
            x => Assert.Equal(DesignatorComparisonStatus.Unchanged, x.DesignatorComparisonStatus));
    }
    

    [Fact]
    public void NewBomLineInTargetHasComparisonStatusOfAdded()
    {
        var comparer = new BomComparisonService();
        
        var sourceBomLines = new List<BomLine>();
        
        var targetBomLines = new List<BomLine>
        {
            new BomLine
            {
                Quantity = 1,
                PartNumber = "321",
                Designators = new List<Designator>
                {
                    new Designator { Name = "R1" },
                    new Designator { Name = "R3" },
                    new Designator { Name = "R4" },
                }
            }
        };
        
        var sourceBom = new Bom
        {
            BomLines = sourceBomLines
        };
        
        var targetBom = new Bom
        {
            BomLines = targetBomLines
        };
        
        var result = comparer.CompareBom(sourceBom, targetBom);
        Assert.Single(result.ComparedBomLines);
        Assert.Equal(BomLineComparisonStatus.Added, result.ComparedBomLines[0].ComparisonStatus);
    }

    [Fact]
    public void RemovedBomLineInTargetHasComparisonStatusOfRemoved()
    {
        var comparer = new BomComparisonService();
        
        var sourceBomLines = new List<BomLine>
        {
            new BomLine
            {
                Quantity = 1,
                PartNumber = "321",
                Designators = new List<Designator>
                {
                    new Designator { Name = "R1" },
                    new Designator { Name = "R3" },
                    new Designator { Name = "R4" },
                }
            }
        };

        var targetBomLines = new List<BomLine>();
        
        var sourceBom = new Bom
        {
            BomLines = sourceBomLines
        };
        
        var targetBom = new Bom
        {
            BomLines = targetBomLines
        };
        
        var result = comparer.CompareBom(sourceBom, targetBom);
        Assert.Single(result.ComparedBomLines);
        Assert.Equal(BomLineComparisonStatus.Removed, result.ComparedBomLines[0].ComparisonStatus);
    }

    [Fact]
    public void ComparingDifferentPartsThrowsException()
    {
        var comparer = new BomComparisonService();
        var source = new BomLine
        {
            PartNumber = "123",
        };
        
        var target = new BomLine
        {
            PartNumber = "321"
        };
        
        Assert.Throws<ArgumentException>(() => comparer.CompareBomLine(source, target));
    }
    
    [Fact]
    public void CompareToNullThrowsArgumentNull()
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

        Assert.Throws<ArgumentNullException>(() => comparer.CompareBomLine(source, null));
        Assert.Throws<ArgumentNullException>(() => comparer.CompareBomLine(null, source));
        Assert.Throws<ArgumentNullException>(() => comparer.CompareBomLine(null, null));
    }
}