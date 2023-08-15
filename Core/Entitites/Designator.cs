namespace Core;

public class Designator
{
    public string Name { get; set; }
    public ComparisonStatus ComparisonStatus { get; set; } = ComparisonStatus.NotCompared;
}