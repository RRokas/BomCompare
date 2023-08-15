using Core.Entitites;
using Xunit;

namespace Core.Tests;

public class BomComparisonTests
{
    [Fact]
    public void NewPartInTarget()
    {
        var comparer = new BomComparisonService();
        var partInBoth = new BomLine
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
        
        var partOnlyInTarget = new BomLine
        {
            Quantity = 1,
            PartNumber = "321",
            Designators = new List<Designator>
            {
                new Designator { Name = "R1" },
                new Designator { Name = "R3" },
                new Designator { Name = "R4" },
            }
        };
        
        var sourceBom = new List<BomLine>
        {
            partInBoth
        };
        
        var targetBom = new List<BomLine>
        {
            partInBoth,
            partOnlyInTarget
        };
        
        var result = comparer.CompareBom(sourceBom, targetBom);
        Assert.Equal(2, result.Count);
    }
}