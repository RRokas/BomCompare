using Core.Entitites;
using Core.ExcelHandling.Npoi;
using Core.Tests.Utilities;

namespace Core.Tests;

public class NpoiExcelTests
{
    [Fact]
    public void ReadValidXlsxBom()
    {
        var excelFile = TestDataDirectory.GetFile("2_lines.xlsx");
        var excelService = new NpoiExcel();
        var bomLines = excelService.ReadBom(excelFile).BomLines;

        Assert.Equal(2, bomLines.Count);

        Assert.Equal(1, bomLines[0].Quantity);
        Assert.Equal("12345", bomLines[0].PartNumber);
        Assert.Equal("A1200", bomLines[0].Designators[0].Name);
        Assert.Equal("A1201", bomLines[0].Designators[1].Name);
        Assert.Equal("10nF", bomLines[0].Value);
        Assert.Equal("Yes", bomLines[0].SMD);
        Assert.Equal("GPS module", bomLines[0].Description);
        Assert.Equal("Contoso", bomLines[0].Manufacturer);
        Assert.Equal("GPS101", bomLines[0].ManufacturerPartNumber);
        Assert.Equal("", bomLines[0].Distributor);
        Assert.Equal("DISTCAP20", bomLines[0].DistributorPartNumber);

        Assert.Equal(2, bomLines[1].Quantity);
        Assert.Equal("12346", bomLines[1].PartNumber);
        Assert.Equal("C1000", bomLines[1].Designators[0].Name);
        Assert.Equal("C1001", bomLines[1].Designators[1].Name);
        Assert.Equal("47uFx10V", bomLines[1].Value);
        Assert.Equal("No", bomLines[1].SMD);
        Assert.Equal("Capacitor", bomLines[1].Description);
        Assert.Equal("NotContoso", bomLines[1].Manufacturer);
        Assert.Equal("CAP20", bomLines[1].ManufacturerPartNumber);
        Assert.Equal("DistrubutorName", bomLines[1].Distributor);
        Assert.Equal("", bomLines[1].DistributorPartNumber);
    }


    [Fact]
    public void ReadValidXlsBom()
    {
        var excelFile = TestDataDirectory.GetFile("2_lines.xls");
        var excelService = new NpoiExcel();
        var bomLines = excelService.ReadBom(excelFile).BomLines;

        Assert.Equal(2, bomLines.Count);

        Assert.Equal(1, bomLines[0].Quantity);
        Assert.Equal("12345", bomLines[0].PartNumber);
        Assert.Equal("A1200", bomLines[0].Designators[0].Name);
        Assert.Equal("A1201", bomLines[0].Designators[1].Name);
        Assert.Equal("10nF", bomLines[0].Value);
        Assert.Equal("Yes", bomLines[0].SMD);
        Assert.Equal("GPS module", bomLines[0].Description);
        Assert.Equal("Contoso", bomLines[0].Manufacturer);
        Assert.Equal("GPS101", bomLines[0].ManufacturerPartNumber);
        Assert.Equal("", bomLines[0].Distributor);
        Assert.Equal("DISTCAP20", bomLines[0].DistributorPartNumber);

        Assert.Equal(2, bomLines[1].Quantity);
        Assert.Equal("12346", bomLines[1].PartNumber);
        Assert.Equal("C1000", bomLines[1].Designators[0].Name);
        Assert.Equal("C1001", bomLines[1].Designators[1].Name);
        Assert.Equal("47uFx10V", bomLines[1].Value);
        Assert.Equal("No", bomLines[1].SMD);
        Assert.Equal("Capacitor", bomLines[1].Description);
        Assert.Equal("NotContoso", bomLines[1].Manufacturer);
        Assert.Equal("CAP20", bomLines[1].ManufacturerPartNumber);
        Assert.Equal("DistrubutorName", bomLines[1].Distributor);
        Assert.Equal("", bomLines[1].DistributorPartNumber);
    }

    [Fact]
    public void WriteComparisonToFile()
    {
        var comparer = new BomComparisonService();

        var sourceBom = new Bom()
        {
            BomLines = new List<BomLine>
            {
                new BomLine
                {
                    Quantity = 1,
                    PartNumber = "321",
                    Designators = new List<Designator>
                    {
                        new Designator
                        {
                            Name = "R1"
                        },
                        new Designator
                        {
                            Name = "R3"
                        },
                        new Designator
                        {
                            Name = "R4"
                        },
                    },
                    Value = "SomeValue",
                    SMD = "Yes",
                    Description = "SomeDescription",
                    Manufacturer = "Contoso",
                    ManufacturerPartNumber = "ContosoPartNumber",
                    Distributor = "ContosoDistributor",
                    DistributorPartNumber = "DistriubtorPartNumber"
                }
            }
        };

        var targetBom = new Bom()
        {
            BomLines = new List<BomLine>
            {
                new BomLine
                {
                    Quantity = 1,
                    PartNumber = "321",
                    Designators = new List<Designator>
                    {
                        new Designator { Name = "R1" },
                        new Designator { Name = "R4" },
                    },
                    Value = "SomeValue",
                    SMD = "Yes",
                    Description = "SomeDescription",
                    Manufacturer = "Contoso",
                    ManufacturerPartNumber = "ContosoPartNumber",
                    Distributor = "ContosoDistributor",
                    DistributorPartNumber = "DistriubtorPartNumber"
                },
                new BomLine
                {
                    Quantity = 1,
                    PartNumber = "111B",
                    Designators = new List<Designator>
                    {
                        new Designator { Name = "T1" },
                        new Designator { Name = "T3" },
                        new Designator { Name = "T4" },
                    }
                }
            }
        };

        var result = comparer.CompareBom(sourceBom, targetBom);
        var comparedBomLines = result.ComparedBomLines;

        var excel = new NpoiExcel();
        excel.WriteBomComparisonToFile("Write_Test.xlsx", result);

        var readFromFile = excel.ReadBom(new FileInfo("Write_Test.xlsx")).BomLines;
        Assert.Equal(comparedBomLines.Count, readFromFile.Count);
        Assert.Equal(comparedBomLines[0].PartNumber, readFromFile[0].PartNumber);
        Assert.Equal(comparedBomLines[0].Quantity, readFromFile[0].Quantity);
        Assert.Equal(comparedBomLines[0].Designators.Count, readFromFile[0].Designators.Count);
        Assert.Equal(comparedBomLines[0].Designators[0].Name, readFromFile[0].Designators[0].Name);
        Assert.Equal(comparedBomLines[0].Designators[1].Name, readFromFile[0].Designators[1].Name);
        Assert.Equal(comparedBomLines[0].Designators[2].Name, readFromFile[0].Designators[2].Name);
        Assert.Equal(comparedBomLines[0].Value, readFromFile[0].Value);
        Assert.Equal(comparedBomLines[0].SMD, readFromFile[0].SMD);
        Assert.Equal(comparedBomLines[0].Description, readFromFile[0].Description);
        Assert.Equal(comparedBomLines[0].Manufacturer, readFromFile[0].Manufacturer);
        Assert.Equal(comparedBomLines[0].ManufacturerPartNumber, readFromFile[0].ManufacturerPartNumber);
        Assert.Equal(comparedBomLines[0].Distributor, readFromFile[0].Distributor);
        Assert.Equal(comparedBomLines[0].DistributorPartNumber, readFromFile[0].DistributorPartNumber);
    }

    [Fact]
    public void WriteComparisonToStream()
    {
        var comparer = new BomComparisonService();

        var sourceBom = new Bom()
        {
            BomLines = new List<BomLine>
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
                    },
                    Value = "SomeValue",
                    SMD = "Yes",
                    Description = "SomeDescription",
                    Manufacturer = "Contoso",
                    ManufacturerPartNumber = "ContosoPartNumber",
                    Distributor = "ContosoDistributor",
                    DistributorPartNumber = "DistriubtorPartNumber"
                }
            }
        };

        var targetBom = new Bom()
        {
            BomLines = new List<BomLine>
            {
                new BomLine
                {
                    Quantity = 1,
                    PartNumber = "321",
                    Designators = new List<Designator>
                    {
                        new Designator { Name = "R1" },
                        new Designator { Name = "R4" },
                    }
                },
                new BomLine
                {
                    Quantity = 1,
                    PartNumber = "111B",
                    Designators = new List<Designator>
                    {
                        new Designator { Name = "T1" },
                        new Designator { Name = "T3" },
                        new Designator { Name = "T4" },
                    }
                }
            }
        };

        var result = comparer.CompareBom(sourceBom, targetBom);
        var comparedBomLines = result.ComparedBomLines;

        var excel = new NpoiExcel();
        var comparisonExcelStream = excel.WriteBomComparisonToStream(result);

        var readFromStream = excel.ReadBom(comparisonExcelStream, "test.xlsx").BomLines;
        Assert.Equal(comparedBomLines.Count, readFromStream.Count);
        Assert.Equal(comparedBomLines[0].PartNumber, readFromStream[0].PartNumber);
        Assert.Equal(comparedBomLines[0].Quantity, readFromStream[0].Quantity);
        Assert.Equal(comparedBomLines[0].Designators.Count, readFromStream[0].Designators.Count);
        Assert.Equal(comparedBomLines[0].Designators[0].Name, readFromStream[0].Designators[0].Name);
        Assert.Equal(comparedBomLines[0].Designators[1].Name, readFromStream[0].Designators[1].Name);
        Assert.Equal(comparedBomLines[0].Designators[2].Name, readFromStream[0].Designators[2].Name);
        Assert.Equal(comparedBomLines[0].Value, readFromStream[0].Value);
        Assert.Equal(comparedBomLines[0].SMD, readFromStream[0].SMD);
        Assert.Equal(comparedBomLines[0].Description, readFromStream[0].Description);
        Assert.Equal(comparedBomLines[0].Manufacturer, readFromStream[0].Manufacturer);
        Assert.Equal(comparedBomLines[0].ManufacturerPartNumber, readFromStream[0].ManufacturerPartNumber);
        Assert.Equal(comparedBomLines[0].Distributor, readFromStream[0].Distributor);
        Assert.Equal(comparedBomLines[0].DistributorPartNumber, readFromStream[0].DistributorPartNumber);
    }
}