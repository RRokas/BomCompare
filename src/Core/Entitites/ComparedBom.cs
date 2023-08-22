namespace Core.Entitites;

public class ComparedBom
{
    public List<ComparedBomLine> ComparedBomLines { get; set; }
    public Bom SourceBom { get; set; }
    public Bom TargetBom { get; set; }
}