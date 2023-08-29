using Core.Entitites;
using NPOI.SS.UserModel;

namespace Core.ExcelHandling.Npoi;

public static class NpoiExtensions
{
    public static void AutoSizeCellColumnWidth(this ICell cell)
    {
        var currentWidth = cell.Sheet.GetColumnWidth(cell.ColumnIndex);
        var cellWidthInCharacters = cell.ToString().Length;
        var cellWidthInNpoiUnits = cellWidthInCharacters * 256;

        if (currentWidth < cellWidthInNpoiUnits)
            cell.Sheet.SetColumnWidth(cell.ColumnIndex, cellWidthInNpoiUnits);
    }
}