using System.Collections;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Core;

public class NpoiExcel : IExcel
{
    public List<BomLine> ReadBom(string path)
    {
        var bom = new List<BomLine>();
        using var file = new FileStream(path, FileMode.Open, FileAccess.Read);
        var workbook = new XSSFWorkbook(file);
        var sheet = workbook.GetSheetAt(0);
        var columnIndexes = GetColumnIndexes(sheet);
        var rowCount = sheet.LastRowNum;

        for (var i = 1; i <= rowCount; i++)
        {
            var row = sheet.GetRow(i);
            var bomLine = new BomLine();

            foreach (var columnIndex in columnIndexes)
            {
                var key = columnIndex.Keys.First();
                var value = columnIndex.Values.First();
                var cell = row.GetCell(value);

                if (key == nameof(BomLine.Quantity)) 
                {
                    bomLine.Quantity = (int)cell.NumericCellValue;
                } 
                else if (key == nameof(BomLine.Designators)) 
                {
                    bomLine.Designators = cell.StringCellValue.Split(", ").ToList();
                } 
                else 
                {
                    var property = typeof(BomLine).GetProperty(key);
                    if (property != null && property.PropertyType == typeof(string)) 
                    {
                        property.SetValue(bomLine, cell.StringCellValue);
                    }
                }
            }
            bom.Add(bomLine);
        }

        return bom;
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("BOM");
        var headerRow = sheet.CreateRow(0);

        // Create header rows from ExcelColumns
        if (ExcelColumns.All != null)
        {
            var headers = ExcelColumns.All.Select(x => x.HeaderName).ToList();
            for (int i = 0; i < headers.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }
        }

        // Access BomLine properties by property name from ExcelColumns
        foreach (var bomLine in bom)
        {
            var row = sheet.CreateRow(sheet.LastRowNum + 1);

            var properties = bomLine.GetType().GetProperties();
            var iterationCounter = 0;
            foreach (var property in properties)
            {
                var cell = row.CreateCell(iterationCounter);
                var value = properties[iterationCounter].GetValue(bomLine, null);

                if (value is ICollection positions)
                {
                    cell.SetCellValue(string.Join(", ", positions));
                }
                else
                {
                    cell.SetCellValue(Convert.ToString(value));
                }

                iterationCounter++;
            }
        }

        workbook.Write(file);
    }
    
    private List<Dictionary<string, int>> GetColumnIndexes(ISheet sheet)
    {
        var columnIndexes = new List<Dictionary<string, int>>();
        var headerRow = sheet.GetRow(0);
        var headerRowCells = headerRow.Cells;

        for (var i = 0; i < headerRowCells.Count; i++)
        {
            var cellValue = headerRowCells[i].StringCellValue;

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