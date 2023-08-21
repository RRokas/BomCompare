using Core.Entitites;

namespace Core;

public interface IExcelWriter
{
    public void WriteBomToFile(string path, List<ComparedBomLine> bom);
    public Stream WriteBomToStream(List<ComparedBomLine> bom);
}