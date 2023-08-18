using System.Diagnostics;
using Core.Attributes;
using Core.Entitites;
using Sylvan.Data.Excel;

namespace Core;

public class SylvanExcelReader
{
    public List<BomLine> ReadBom(string path)
    {
        using ExcelDataReader edr = ExcelDataReader.Create(path);
        var bom = new List<BomLine>();
        var headerIndexes = ReadHeaderIndexes(edr);
        var properties = typeof(BomLine).GetProperties();
        
        while (edr.Read())
        {
            var bomLine = new BomLine();
            foreach (var property in properties)
            {
                var columnNames = (ExcelColumnName[])property.GetCustomAttributes(typeof(ExcelColumnName), false);
                if (columnNames.Length > 0)
                {
                    var columnName = columnNames[0].ColumnName;
                    if (headerIndexes.ContainsKey(columnName))
                    {
                        var columnIndex = headerIndexes[columnName];
                        var cell = edr[columnIndex];
                        if(cell is DBNull)
                            continue;
                        
                        if (property.PropertyType == typeof(string)) 
                        {
                            property.SetValue(bomLine, cell);
                        }
                        else if (property.PropertyType == typeof(int)) 
                        {
                            bomLine.Quantity = Convert.ToInt32(cell);
                        } 
                        else if (property.PropertyType == typeof(List<Designator>))
                        {
                            var desigs = cell
                                .ToString()
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

    public List<BomLine> ReadBom(Stream stream)
    {
        throw new NotImplementedException();
    }

    private Dictionary<string, int> ReadHeaderIndexes(ExcelDataReader edr)
    {
        var headerIndexes = new Dictionary<string, int>();
        for (var i = 0; i < edr.FieldCount; i++)
        {
            headerIndexes.Add(edr.GetString(i), i);
        }

        return headerIndexes;
    }
}