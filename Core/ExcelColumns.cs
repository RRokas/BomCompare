using System.Reflection;

namespace Core;

public static class ExcelColumns
{
    public const string Quantity = "Quantity";
    public const string PartNumber = "Part Number";
    public const string Designators = "Designator";
    public const string Value = "Value";
    public const string Smd = "SMD";
    public const string Description = "Description";
    public const string Manufacturer = "Manufacturer";
    public const string ManufacturerPartNumber = "Manufacturer Part Number";
    public const string Distributor = "Distributor";
    public const string DistributorPartNumber = "Distributor Part Number";
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