using System.Collections.ObjectModel;
using System.Data.Common;
using Core;
using Sylvan.Data;
using Sylvan.Data.Excel;

public class SylvanExcel : IExcel
{
    public List<BomLine> ReadBom(string path)
    {
        using var edr = ExcelDataReader.Create(path);
        var bom = new List<BomLine>();
        var columnIndexes = GetColumnIndexes(edr.GetColumnSchema());

        while (edr.Read())
        {
            var bomLine = new BomLine();

            foreach (var columnIndex in columnIndexes)
            {
                var key = columnIndex.Key;
                var value = columnIndex.Value;
                var property = typeof(BomLine).GetProperty(key);

                if (key == nameof(BomLine.Quantity)) 
                {
                    bomLine.Quantity = (int)edr.GetDouble(value);
                } 
                else if (key == nameof(BomLine.Designators)) 
                {
                    bomLine.Designators = edr.GetString(value).Split(", ").ToList()
                        .Select(x => new Designator { Name = x, ComparisonStatus = ComparisonStatus.NotCompared }).ToList();;
                } 
                else 
                {
                    if (property != null && property.PropertyType == typeof(string)) 
                    {
                        property.SetValue(bomLine, edr.GetString(value));
                    }
                }
            }
            bom.Add(bomLine);
        }

        return bom;
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        using var excelDataWriter = ExcelDataWriter.Create(path);

        var builder = new ObjectDataReader.Builder<BomLine>();

        var properties = typeof(BomLine).GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(List<string>))
            {
                builder.AddColumn(property.Name, x => string.Join(", ", (List<string>) property.GetValue(x)));
            }
            else
            {
                builder.AddColumn(property.Name, x => property.GetValue(x)?.ToString());
            }
        }

        excelDataWriter.Write(builder.Build(bom));
    }

    private static Dictionary<string, int> GetColumnIndexes(ReadOnlyCollection<DbColumn> headers)
    {
        var columnIndexes = new Dictionary<string, int>();

        foreach (var columnDefinition in Core.ExcelColumns.All)
        {
            var header = headers.FirstOrDefault(h => h.ColumnName == columnDefinition.HeaderName);
            if (header != null)
            {
                columnIndexes.Add(columnDefinition.FieldName, header.ColumnOrdinal.Value);
            }
        }

        return columnIndexes;
    }
}