using Sylvan.Data;
using Sylvan.Data.Excel;

namespace Core.SylvenImplementation;

public class SylvanExcel : IExcel
{
    public List<BomLine> ReadBom(string path)
    {
        using var edr = ExcelDataReader.Create(path);
        return edr.GetRecords<BomLine>().ToList();
    }

    public void WriteBom(string path, List<BomLine> bom)
    {
        throw new NotImplementedException();
    }
}