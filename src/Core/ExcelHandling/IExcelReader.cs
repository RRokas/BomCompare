using Core.Entitites;

namespace Core;

public interface IExcelReader
{
    public Bom ReadBom(FileInfo path);
    public Bom ReadBom(Stream stream, string filename);
}