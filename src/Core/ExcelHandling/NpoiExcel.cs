using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Core.Attributes;
using Core.Entitites;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Core;

public class NpoiExcel : IExcelReader, IExcelWriter
{
    public List<BomLine> ReadBom(string path)
    {
        using var file = new FileStream(path, FileMode.Open, FileAccess.Read);
        return ReadBom(file);
    }

    public List<BomLine> ReadBom(Stream stream)
    {
        var bom = new List<BomLine>();
        var workbook = new XSSFWorkbook(stream);
        var sheet = workbook.GetSheetAt(0);
        var headers = sheet.GetRow(0).Cells.Select(cell => cell.StringCellValue).ToList();
        var rowCount = sheet.LastRowNum;
        var properties = typeof(BomLine).GetProperties();
        
        for (var i = 1; i <= rowCount; i++)
        {
            var row = sheet.GetRow(i);
            var bomLine = new BomLine();
            

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
                                .Select(x => new Designator(x, DesignatorComparisonStatus.NotCompared))
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

    public void WriteBom(string path, List<ComparedBomLine> bom)
    {
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("BOM_comparison");

        // Create header rows from ExcelColumnName Attributes
        var properties = typeof(ComparedBomLine).GetProperties();
        CreateHeader(sheet, properties);
        sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, properties.Length - 1));
        
        foreach (var bomLine in bom)
        {
            CreateBomLine(sheet, bomLine, properties);
        }

        workbook.Write(file);
    }

    private void CreateHeader(ISheet sheet, PropertyInfo[] properties)
    {
        var headerRow = sheet.CreateRow(0);
        
        var headerCellStyle = CreateHeaderCellStyle(sheet);
        
        for (int i = 0; i < properties.Length; i++)
        {
            var prop = properties[i];
            var attr = ((ExcelColumnName[])prop.GetCustomAttributes(typeof(ExcelColumnName), false)).FirstOrDefault();

            if (attr != null)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(attr.ColumnName);
                cell.CellStyle = headerCellStyle;
            }
        }
    }

    private ICellStyle CreateHeaderCellStyle(ISheet sheet)
    {
        var cellStyle = sheet.Workbook.CreateCellStyle();
        var font = sheet.Workbook.CreateFont();
        
        font.IsBold = true;
        font.FontHeightInPoints = 10;
        cellStyle.SetFont(font);
        
        return cellStyle;
    }
    
    private void CreateCellAndSetValue(IRow row, int cellIndex, object cellValue)
    {
        var cell = row.CreateCell(cellIndex);
        switch(cellValue)
        {
            case List<string> strList:
                cell.SetCellValue(string.Join(", ", strList));
                break;

            case List<Designator> designators:
                WriteDesignatorCellWithFormatting(cell, designators);
                break;
            
            case int intValue:
                cell.SetCellValue(intValue);
                break;
            
            default:
                cell.SetCellValue(Convert.ToString(cellValue));
                break;
        }
    }

    private void WriteDesignatorCellWithFormatting(ICell cell, List<Designator> designators)
    {
        var richTextString = new XSSFRichTextString();
        for (var index = 0; index < designators.Count; index++)
        {
            var designator = designators[index];
            var font = GetFontStyleForDesignator(designator);
            
            richTextString.Append(designator.Name, font);
            
            if(designators.Count - 1 != index)
                richTextString.Append(", ");
        }

        cell.SetCellValue(richTextString);
    }

    private XSSFFont GetFontStyleForDesignator(Designator designator)
    {
        var font = new XSSFFont();
        
        font.Color = designator.DesignatorComparisonStatus switch
        {
            DesignatorComparisonStatus.Added => HSSFColor.Green.Index,
            DesignatorComparisonStatus.Removed => HSSFColor.Red.Index,
            _ => font.Color
        };
        
        if(designator.DesignatorComparisonStatus == DesignatorComparisonStatus.Removed)
            font.IsStrikeout = true;
        
        return font;
    }
    
    private void CreateBomLine(ISheet sheet, BomLine bomLine, PropertyInfo[] properties)
    {
        var row = sheet.CreateRow(sheet.LastRowNum + 1);
        
        for (int i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            var attr = (ExcelColumnName[])property.GetCustomAttributes(typeof(ExcelColumnName), false);

            if (!attr.Any()) continue;

            var value = property.GetValue(bomLine, null);
            CreateCellAndSetValue(row, i, value);
        }
    }
}