using Sylvan.Data.Excel;

namespace Core.Sylvan;

public class SylvanExcel : IExcel
{
    public List<BomLine> ReadBom(string path)
    {
        using ExcelDataReader edr = ExcelDataReader.Create("data.xls");
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        throw new NotImplementedException();
    }
}