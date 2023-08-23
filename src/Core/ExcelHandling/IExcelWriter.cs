using Core.Entitites;

namespace Core.ExcelHandling;

public interface IExcelWriter
{
    public void WriteBomComparisonToFile(string path, ComparedBom comparedBom);
    public Stream WriteBomComparisonToStream(ComparedBom comparedBom);
}