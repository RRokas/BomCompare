using Core.Entitites;

namespace Core;

public interface IExcelWriter
{
    public void WriteBomToFile(string path, ComparedBom comparedBom);
    public Stream WriteBomToStream(ComparedBom comparedBom);
}