using System.ComponentModel;
using Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CLI;

public class CompareBom : Command<CompareBomComandSettings>
{
    public override int Execute(CommandContext context, CompareBomComandSettings settings)
    {
        var excel = new NpoiExcel();
        var source = excel.ReadBom(settings.Source);
        var target = excel.ReadBom(settings.Target);
        var comparer = new BomComparisonService();
        var result = comparer.CompareBom(source, target);
        AnsiConsole.WriteLine($"Compared item count: {result.Count}");
        excel.WriteBom(settings.Output, result);
        return 0;
    }
}