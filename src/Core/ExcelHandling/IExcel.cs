namespace Core;

public interface IExcel
{
    public List<BomLine> ReadBom(string path);
    public void WriteBom(string path, List<BomLine> bom, bool includeComparisonStatus);
}