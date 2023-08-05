using System.Reflection;

namespace Core;

public static class ExcelColumns
{
    public const string InternalPartId = "Internal Part ID";
    public const string ManufacturerPartId = "Manufacturer Part ID";
    public const string ManufacturerName = "Manufacturer Name";
    public const string PartDescription = "Part Description";
    public const string Quantity = "Quantity";
    public const string Value = "Value";
    public const string Positions = "Positions";

    private static List<(string FieldName, string HeaderName)>? _all;  
    
    public static IReadOnlyList<(string FieldName, string HeaderName)>? All 
    {
        get 
        {
            if (_all != null) return _all;
            
            _all = typeof(ExcelColumns)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(x => (x.Name, x.GetValue(null).ToString()))
                .ToList();
                
            return _all;
        }
    }
}