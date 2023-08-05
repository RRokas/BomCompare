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

                switch (key)
                {
                    case nameof(BomLine.InternalPartId):
                        bomLine.InternalPartId = cell.StringCellValue;
                        break;
                    case nameof(BomLine.ManufacturerPartId):
                        bomLine.ManufacturerPartId = cell.StringCellValue;
                        break;
                    case nameof(BomLine.ManufacturerName):
                        bomLine.ManufacturerName = cell.StringCellValue;
                        break;
                    case nameof(BomLine.PartDescription):
                        bomLine.PartDescription = cell.StringCellValue;
                        break;
                    case nameof(BomLine.Quantity):
                        bomLine.Quantity = (int)cell.NumericCellValue;
                        break;
                    case nameof(BomLine.Value):
                        bomLine.Value = cell.StringCellValue;
                        break;
                    case nameof(BomLine.Positions):
                        bomLine.Positions = cell.StringCellValue.Split(',').ToList();
                        break;
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

            row.CreateCell(0).SetCellValue(bomLine.InternalPartId);
            row.CreateCell(1).SetCellValue(bomLine.ManufacturerPartId);
            row.CreateCell(2).SetCellValue(bomLine.ManufacturerName);
            row.CreateCell(3).SetCellValue(bomLine.PartDescription);
            row.CreateCell(4).SetCellValue(bomLine.Quantity);
            row.CreateCell(5).SetCellValue(bomLine.Value);
            row.CreateCell(6).SetCellValue(string.Join(",", bomLine.Positions));
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