using Core.Entitites;

namespace Core.ExcelHandling;

public interface IExcelReader
{
    public Bom ReadBom(FileInfo path);
    public Bom ReadBom(Stream stream, string filename);
}