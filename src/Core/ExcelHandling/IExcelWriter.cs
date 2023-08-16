using Core.Entitites;

namespace Core;

public interface IExcelWriter
{
    public void WriteBom(string path, List<ComparedBomLine> bom);
}