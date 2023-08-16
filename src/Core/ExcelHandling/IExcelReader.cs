using Core.Entitites;

namespace Core;

public interface IExcelReader
{
    public List<BomLine> ReadBom(string path);
}