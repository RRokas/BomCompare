using NPOI.SS.UserModel;

namespace Core;

public static class NpoiExtensions
{
    // extension for ICell to get the value as string
    public static string AnyCellTypeAsString(this ICell cell)
    {
        if (cell == null)
            return string.Empty;
        
        switch (cell.CellType)
        {
            case CellType.Numeric:
                return cell.NumericCellValue.ToString();
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue.ToString();
            case CellType.Formula:
                return cell.CellFormula;
            case CellType.Blank:
                return string.Empty;
            case CellType.Error:
                return cell.ErrorCellValue.ToString();
            default:
                return string.Empty;
        }
    }
}