using Core;
using Core.ExcelHandling.Npoi;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CLI;

public class CompareBom : Command<CompareBomComandSettings>
{
    public override int Execute(CommandContext context, CompareBomComandSettings settings)
    {
        var excel = new NpoiExcel();
        var source = excel.ReadBom(new FileInfo(settings.Source));
        var target = excel.ReadBom(new FileInfo(settings.Target));
        var comparer = new BomComparisonService();
        var result = comparer.CompareBom(source, target);
        AnsiConsole.WriteLine($"Compared item count: {result.ComparedBomLines.Count}");
        excel.WriteBomComparisonToFile(settings.Output, result);
        return 0;
    }
}