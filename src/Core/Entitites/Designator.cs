namespace Core.Entitites;

public class Designator
{
    public string Name { get; set; }
    public ComparisonStatus ComparisonStatus { get; set; }

    public Designator()
    {
        Name = string.Empty;
        ComparisonStatus = ComparisonStatus.NotCompared;
    }
    
    public Designator(string name, ComparisonStatus comparisonStatus)
    {
        Name = name;
        ComparisonStatus = comparisonStatus;
    }
}