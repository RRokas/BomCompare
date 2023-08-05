using System.Collections.ObjectModel;
using System.Data.Common;
using Sylvan.Data;
using Sylvan.Data.Excel;

namespace Core.SylvanImplementation;

public class SylvanExcel : IExcel
{
    public List<BomLine> ReadBom(string path)
    {
        using var edr = ExcelDataReader.Create(path);
        var bom = new List<BomLine>();
        var columnIndexes = GetColumIndexes(edr.GetColumnSchema());
        while (edr.Read())
        {
            bom.Add(new BomLine
            {
                InternalPartId = edr.GetString(columnIndexes[nameof(BomLine.InternalPartId)]),
                ManufacturerPartId = edr.GetString(columnIndexes[nameof(BomLine.ManufacturerPartId)]),
                ManufacturerName = edr.GetString(columnIndexes[nameof(BomLine.ManufacturerName)]),
                PartDescription = edr.GetString(columnIndexes[nameof(BomLine.PartDescription)]),
                Quantity = edr.GetDouble(columnIndexes[nameof(BomLine.Quantity)]),
                Value = edr.GetString(columnIndexes[nameof(BomLine.Value)]),
                Positions = edr.GetString(columnIndexes[nameof(BomLine.Positions)]).Split(',').ToList()
            });
        }
        return bom;
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        using var excelDataWriter = ExcelDataWriter.Create(path);
        
        var builder = new ObjectDataReader.Builder<BomLine>()
            .AddColumn(ExcelColumns.InternalPartId, x => x.InternalPartId)
            .AddColumn(ExcelColumns.ManufacturerPartId, x => x.ManufacturerPartId)
            .AddColumn(ExcelColumns.ManufacturerName, x => x.ManufacturerName)
            .AddColumn(ExcelColumns.PartDescription, x => x.PartDescription)
            .AddColumn(ExcelColumns.Quantity, x => x.Quantity)
            .AddColumn(ExcelColumns.Value, x => x.Value)
            .AddColumn(ExcelColumns.Positions, x => string.Join(",", x.Positions));

        excelDataWriter.Write(builder.Build(bom));
    }

    private static Dictionary<string, int> GetColumIndexes(ReadOnlyCollection<DbColumn> headers)
    {
        var columnIndexes = new Dictionary<string, int>();

        foreach (var columnDefinition in ExcelColumns.All)  
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