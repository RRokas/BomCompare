namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnName : Attribute
{
    public string ColumnName { get; }

    public ExcelColumnName(string columnName)
    {
        ColumnName = columnName;
    }
}