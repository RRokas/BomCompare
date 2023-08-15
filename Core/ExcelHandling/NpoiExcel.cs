using System.Collections;
using System.Reflection;
using Core.Attributes;
using Core.Entitites;
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
        var headers = sheet.GetRow(0).Cells.Select(cell => cell.StringCellValue).ToList();
        var rowCount = sheet.LastRowNum;

        for (var i = 1; i <= rowCount; i++)
        {
            var row = sheet.GetRow(i);
            var bomLine = new BomLine();
            var properties = typeof(BomLine).GetProperties();

            foreach (var property in properties)
            {
                var attrs = (ExcelColumnName[])property.GetCustomAttributes(typeof(ExcelColumnName), false);

                if (attrs.Length > 0)
                {
                    var column = headers.IndexOf(attrs[0].ColumnName);
                    if(column != -1)
                    {
                        var cell = row.GetCell(column);
                        if (property.PropertyType == typeof(string)) 
                        {
                            property.SetValue(bomLine, cell.StringCellValue);
                        }
                        else if (property.PropertyType == typeof(int)) 
                        {
                            bomLine.Quantity = (int)cell.NumericCellValue;
                        } 
                        else if (property.PropertyType == typeof(List<Designator>))
                        {
                            var desigs = cell.StringCellValue
                                .Split(", ")
                                .Select(x => new Designator(x, ComparisonStatus.NotCompared))
                                .ToList();

                            property.SetValue(bomLine, desigs);
                        } 
                    }
                }
            }
            bom.Add(bomLine);
        }

        return bom;
    }

    public void WriteBom(string path, List<BomLine> bom, bool includeComparisonStatus)
    {
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("BOM_comparison");

        // Create header rows from ExcelColumnName Attributes
        var properties = typeof(BomLine).GetProperties();
        CreateHeader(sheet, properties, includeComparisonStatus);
        
        foreach (var bomLine in bom)
        {
            CreateBomLine(sheet, bomLine, properties, includeComparisonStatus);
        }

        workbook.Write(file);
    }

    private void CreateHeader(ISheet sheet, PropertyInfo[] properties, bool includeComparisonStatus)
    {
        var headerRow = sheet.CreateRow(0);
        int iterationStartIndex = 0;

        if (includeComparisonStatus)
        {
            headerRow.CreateCell(0).SetCellValue("Comparison Status");
            iterationStartIndex = 1; // start iteration from 1 if 'Comparison Status' is included
        }

        for (int i = 0; i < properties.Length; i++)
        {
            var prop = properties[i];
            var attr = ((ExcelColumnName[])prop.GetCustomAttributes(typeof(ExcelColumnName), false)).FirstOrDefault();

            if (attr != null)
            {
                headerRow.CreateCell(i + iterationStartIndex).SetCellValue(attr.ColumnName);
            }
        }
    }
    
    private void CreateCellAndSetValue(IRow row, int cellIndex, object cellValue)
    {
        var cell = row.CreateCell(cellIndex);
        switch(cellValue)
        {
            case List<string> strList:
                cell.SetCellValue(string.Join(", ", strList));
                break;

            case ICollection positions:
                cell.SetCellValue(string.Join(", ", positions));
                break;

            default:
                cell.SetCellValue(Convert.ToString(cellValue));
                break;
        }
    }
    
    private void CreateBomLine(ISheet sheet, BomLine bomLine, PropertyInfo[] properties, bool includeComparisonStatus)
    {
        var row = sheet.CreateRow(sheet.LastRowNum + 1);

        if (includeComparisonStatus)
        {
            var comparisonResults = bomLine.Designators
                .Select(x => x.ComparisonStatus)
                .Distinct()
                .Select(x => x.ToString())
                .ToList();
            CreateCellAndSetValue(row, 0, comparisonResults);
        }

        int startColumnIndex = includeComparisonStatus ? 1 : 0;
        for (int i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            var attr = (ExcelColumnName[])property.GetCustomAttributes(typeof(ExcelColumnName), false);

            if (!attr.Any()) continue;

            var value = property.GetValue(bomLine, null);
            CreateCellAndSetValue(row, i + startColumnIndex, value);
        }
    }
}