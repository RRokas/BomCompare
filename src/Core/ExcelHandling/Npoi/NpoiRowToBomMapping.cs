using System.Reflection;

namespace Core.ExcelHandling.Npoi;

public class NpoiRowToBomMapping
{
    public PropertyInfo PropertyInfo { get; set; }
    public int IndexInExcel { get; set; }
}