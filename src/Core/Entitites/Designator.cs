namespace Core.Entitites;

public class Designator
{
    public string Name { get; set; }
    public DesignatorComparisonStatus DesignatorComparisonStatus { get; set; }

    public Designator()
    {
        Name = string.Empty;
        DesignatorComparisonStatus = DesignatorComparisonStatus.NotCompared;
    }
    
    public Designator(string name, DesignatorComparisonStatus designatorComparisonStatus)
    {
        Name = name;
        DesignatorComparisonStatus = designatorComparisonStatus;
    }

    public Designator(string name)
    {
        Name = name;
        DesignatorComparisonStatus = DesignatorComparisonStatus.NotCompared;
    }
}