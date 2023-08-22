using Core.Entitites;
using Core.Tests.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Core.Tests;

public class NpoiExcelTests
{
    [Fact]
    public void ReadValidXlsxBom()
    {
        var excelFile = TestDataDirectory.GetFile("2_lines.xlsx");
        var excelService = new NpoiExcel();
        var bom = excelService.ReadBom(excelFile.FullName);

        Assert.Equal(2, bom.Count);

        Assert.Equal(1, bom[0].Quantity);
        Assert.Equal("12345", bom[0].PartNumber);
        Assert.Equal(bom[0].Designators[0].Name, "A1200");
        Assert.Equal(bom[0].Designators[1].Name, "A1201");
        Assert.Equal("10nF", bom[0].Value);
        Assert.Equal("Yes", bom[0].SMD);
        Assert.Equal("GPS module", bom[0].Description);
        Assert.Equal("Contoso", bom[0].Manufacturer);
        Assert.Equal("GPS101", bom[0].ManufacturerPartNumber);
        Assert.Equal("", bom[0].Distributor);
        Assert.Equal("DISTCAP20", bom[0].DistributorPartNumber);

        Assert.Equal(2, bom[1].Quantity);
        Assert.Equal("12346", bom[1].PartNumber);
        Assert.Equal(bom[1].Designators[0].Name, "C1000");
        Assert.Equal(bom[1].Designators[1].Name, "C1001");
        Assert.Equal("47uFx10V", bom[1].Value);
        Assert.Equal("No", bom[1].SMD);
        Assert.Equal("Capacitor", bom[1].Description);
        Assert.Equal("NotContoso", bom[1].Manufacturer);
        Assert.Equal("CAP20", bom[1].ManufacturerPartNumber);
        Assert.Equal("DistrubutorName", bom[1].Distributor);
        Assert.Equal("", bom[1].DistributorPartNumber);
    }


    [Fact]
    public void ReadValidXlsBom()
    {
        var excelFile = TestDataDirectory.GetFile("2_lines.xls");
        var excelService = new NpoiExcel();
        var bom = excelService.ReadBom(excelFile.FullName);

        Assert.Equal(2, bom.Count);

        Assert.Equal(1, bom[0].Quantity);
        Assert.Equal("12345", bom[0].PartNumber);
        Assert.Equal(bom[0].Designators[0].Name, "A1200");
        Assert.Equal(bom[0].Designators[1].Name, "A1201");
        Assert.Equal("10nF", bom[0].Value);
        Assert.Equal("Yes", bom[0].SMD);
        Assert.Equal("GPS module", bom[0].Description);
        Assert.Equal("Contoso", bom[0].Manufacturer);
        Assert.Equal("GPS101", bom[0].ManufacturerPartNumber);
        Assert.Equal("", bom[0].Distributor);
        Assert.Equal("DISTCAP20", bom[0].DistributorPartNumber);

        Assert.Equal(2, bom[1].Quantity);
        Assert.Equal("12346", bom[1].PartNumber);
        Assert.Equal(bom[1].Designators[0].Name, "C1000");
        Assert.Equal(bom[1].Designators[1].Name, "C1001");
        Assert.Equal("47uFx10V", bom[1].Value);
        Assert.Equal("No", bom[1].SMD);
        Assert.Equal("Capacitor", bom[1].Description);
        Assert.Equal("NotContoso", bom[1].Manufacturer);
        Assert.Equal("CAP20", bom[1].ManufacturerPartNumber);
        Assert.Equal("DistrubutorName", bom[1].Distributor);
        Assert.Equal("", bom[1].DistributorPartNumber);
    }

    [Fact]
    public void WriteComparisonToFile()
    {
        var comparer = new BomComparisonService();

        var sourceBom = new List<BomLine>
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
        };

        var targetBom = new List<BomLine>
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
        };

        var result = comparer.CompareBom(sourceBom, targetBom);

        var excel = new NpoiExcel();
        excel.WriteBomToFile("Write_Test.xlsx", result);

        var readFromFile = excel.ReadBom("Write_Test.xlsx");
        Assert.Equal(result.Count, readFromFile.Count);
        Assert.Equal(result[0].PartNumber, readFromFile[0].PartNumber);
        Assert.Equal(result[0].Quantity, readFromFile[0].Quantity);
        Assert.Equal(result[0].Designators.Count, readFromFile[0].Designators.Count);
        Assert.Equal(result[0].Designators[0].Name, readFromFile[0].Designators[0].Name);
        Assert.Equal(result[0].Designators[1].Name, readFromFile[0].Designators[1].Name);
        Assert.Equal(result[0].Designators[2].Name, readFromFile[0].Designators[2].Name);
        Assert.Equal(result[0].Value, readFromFile[0].Value);
        Assert.Equal(result[0].SMD, readFromFile[0].SMD);
        Assert.Equal(result[0].Description, readFromFile[0].Description);
        Assert.Equal(result[0].Manufacturer, readFromFile[0].Manufacturer);
        Assert.Equal(result[0].ManufacturerPartNumber, readFromFile[0].ManufacturerPartNumber);
        Assert.Equal(result[0].Distributor, readFromFile[0].Distributor);
        Assert.Equal(result[0].DistributorPartNumber, readFromFile[0].DistributorPartNumber);
    }

    [Fact]
    public void WriteToStream()
    {
        var comparer = new BomComparisonService();

        var sourceBom = new List<BomLine>
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
        };

        var targetBom = new List<BomLine>
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
        };

        var result = comparer.CompareBom(sourceBom, targetBom);

        var excel = new NpoiExcel();
        var comparisonExcelStream = excel.WriteBomToStream(result);

        var readFromStream = excel.ReadBom(comparisonExcelStream);
        Assert.Equal(result.Count, readFromStream.Count);
        Assert.Equal(result[0].PartNumber, readFromStream[0].PartNumber);
        Assert.Equal(result[0].Quantity, readFromStream[0].Quantity);
        Assert.Equal(result[0].Designators.Count, readFromStream[0].Designators.Count);
        Assert.Equal(result[0].Designators[0].Name, readFromStream[0].Designators[0].Name);
        Assert.Equal(result[0].Designators[1].Name, readFromStream[0].Designators[1].Name);
        Assert.Equal(result[0].Designators[2].Name, readFromStream[0].Designators[2].Name);
        Assert.Equal(result[0].Value, readFromStream[0].Value);
        Assert.Equal(result[0].SMD, readFromStream[0].SMD);
        Assert.Equal(result[0].Description, readFromStream[0].Description);
        Assert.Equal(result[0].Manufacturer, readFromStream[0].Manufacturer);
        Assert.Equal(result[0].ManufacturerPartNumber, readFromStream[0].ManufacturerPartNumber);
        Assert.Equal(result[0].Distributor, readFromStream[0].Distributor);
        Assert.Equal(result[0].DistributorPartNumber, readFromStream[0].DistributorPartNumber);
    }
}