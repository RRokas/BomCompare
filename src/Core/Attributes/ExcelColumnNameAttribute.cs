namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnNameAttribute : Attribute
{
    public string ColumnName { get; }

    public ExcelColumnNameAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}