using System.Reflection;
using Core.Attributes;
using Core.Entitites;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Core;

public class NpoiExcel : IExcelReader, IExcelWriter
{
    public Bom ReadBom(FileInfo path)
    {
        using var file = new FileStream(path.FullName, FileMode.Open, FileAccess.Read);
        return ReadBom(file, path.Name);
    }

    public Bom ReadBom(Stream stream, string filename)
    {
        var bomLines = new List<BomLine>();
        var workbook = WorkbookFactory.Create(stream);
        var sheet = workbook.GetSheetAt(0);
        var headers = sheet.GetRow(0).Cells.Select(cell => cell.StringCellValue).ToList();
        var rowCount = sheet.LastRowNum;
        var properties = typeof(BomLine).GetProperties();
        
        for (var i = 1; i <= rowCount; i++)
        {
            var row = sheet.GetRow(i);
            var bomLine = new BomLine();
            
            if(row == null)
                break;

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
                            property.SetValue(bomLine, cell.ToString());
                        }
                        else if (property.PropertyType == typeof(int)) 
                        {
                            bomLine.Quantity = (int)cell.NumericCellValue;
                        } 
                        else if (property.PropertyType == typeof(List<Designator>))
                        {
                            var desigs = cell.ToString()
                                .Split(new[] { ", ", "," }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .Select(name => new Designator(name))
                                .ToList();

                            property.SetValue(bomLine, desigs);
                        } 
                    }
                }
            }
            bomLines.Add(bomLine);
        }
        
        return new Bom
        {
            BomLines = bomLines,
            FileName = filename
        };
    }

    private XSSFWorkbook CreateBomComparisonWorkbook(ComparedBom comparedBom)
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("BOM_comparison");
        
        var properties = typeof(ComparedBomLine).GetProperties();
        CreateHeader(sheet, properties);
        sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, properties.Length - 1));
        
        foreach (var bomLine in comparedBom.ComparedBomLines)
        {
            CreateBomLine(sheet, bomLine, properties);
        }
        
        WriteComparisonFilenamesToNewSheet(workbook, comparedBom);

        return workbook;
    }

    public void WriteBomToFile(string path, ComparedBom comparedBom)
    {
        var workbook = CreateBomComparisonWorkbook(comparedBom);
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
        workbook.Write(file);
    }

    public Stream WriteBomToStream(ComparedBom bom)
    {
        const bool leaveStreamOpen = true;
        var workbook = CreateBomComparisonWorkbook(bom);
        var stream = new MemoryStream();
        workbook.Write(stream, leaveStreamOpen);
        stream.Position = 0;
        return stream;
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
    
    private void WriteComparisonFilenamesToNewSheet(IWorkbook book, ComparedBom comparedBom)
    {
        var sheet = book.CreateSheet("Comparison_filenames");
        var row = sheet.CreateRow(0);
        
        var sourceCell = row.CreateCell(0);
        sourceCell.SetCellValue("Source filename");
        
        var targetCell = row.CreateCell(1);
        targetCell.SetCellValue("Target filename");
        
        var secondRow = sheet.CreateRow(1);
        
        var sourceFilenameCell = secondRow.CreateCell(0);
        sourceFilenameCell.SetCellValue(comparedBom.SourceBom.FileName);
        
        var targetFilenameCell = secondRow.CreateCell(1);
        targetFilenameCell.SetCellValue(comparedBom.TargetBom.FileName);
        
    }
}