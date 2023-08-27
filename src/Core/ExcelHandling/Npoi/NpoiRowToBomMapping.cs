using System.Reflection;
using Core.Attributes;

namespace Core.ExcelHandling.Npoi;

public class NpoiRowToBomMapping
{
    public PropertyInfo PropertyInfo { get; set; }
    public int IndexInExcel { get; set; }
}