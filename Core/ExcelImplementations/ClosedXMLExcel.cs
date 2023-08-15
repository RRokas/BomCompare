using System.Collections;
using ClosedXML.Excel;

namespace Core;

public class ClosedXMLExcel : IExcel
{
public List<BomLine> ReadBom(string path)
    {
        var bom = new List<BomLine>();
        using var workbook = new XLWorkbook(path);
        var sheet = workbook.Worksheet(1);
        var columnIndexes = GetColumnIndexes(sheet);

        // Setup initial state for while loop
        int rowIndex = 2;
        var row = sheet.Row(rowIndex);
        while (!row.IsEmpty())
        {
            var bomLine = new BomLine();

            foreach (var columnIndex in columnIndexes)
            {
                var key = columnIndex.Keys.First();
                var value = columnIndex.Values.First();
                var cell = row.Cell(value + 1);

                if (key == nameof(BomLine.Quantity))
                {
                    bomLine.Quantity = (int)cell.GetValue<double>();
                }
                else if (key == nameof(BomLine.Designators))
                {
                    bomLine.Designators = 
                        cell.GetValue<string>().Split(", ").Select(x => new Designator { Name = x, ComparisonStatus = ComparisonStatus.NotCompared }).ToList();
                }
                else
                {
                    var property = typeof(BomLine).GetProperty(key);
                    if (property != null && property.PropertyType == typeof(string))
                    {
                        property.SetValue(bomLine, cell.GetValue<string>());
                    }
                }
            }

            bom.Add(bomLine);
        
            // Move to next row
            rowIndex++;
            row = sheet.Row(rowIndex);
        }

        return bom;
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("BOM");
        
        // Create header rows from ExcelColumns
        if (ExcelColumns.All != null)
        {
            var headers = ExcelColumns.All.Select(x => x.HeaderName).ToList();
            for (int i = 0; i < headers.Count; i++)
            {
                sheet.Cell(1, i+1).Value = headers[i];
            }
        }

        // Access BomLine properties by property name from ExcelColumns
        foreach (var bomLine in bom)
        {
            var row = sheet.LastRow().RowBelow();

            var properties = bomLine.GetType().GetProperties();
            for (var j = 0; j < properties.Length; j++)
            {
                var cell = row.Cell(j+1);
                var value = properties[j].GetValue(bomLine);

                if (value is ICollection collection)
                {
                    cell.Value = string.Join(", ", collection);
                }
                else
                {
                    cell.Value = Convert.ToString(value);
                }
            }
        }

        workbook.SaveAs(path);
    }

    private List<Dictionary<string, int>> GetColumnIndexes(IXLWorksheet sheet)
    {
        var columnIndexes = new List<Dictionary<string, int>>();
        var headerRow = sheet.Row(1);

        for (var i = 0; i < headerRow.CellCount(); i++)
        {
            var cellValue = headerRow.Cell(i + 1).GetValue<string>();

            foreach (var column in Core.ExcelColumns.All)
            {
                if (column.HeaderName != cellValue) continue;
                columnIndexes.Add(new Dictionary<string, int> {{column.FieldName, i}});
                break;
            }
        }

        return columnIndexes;
    }
}