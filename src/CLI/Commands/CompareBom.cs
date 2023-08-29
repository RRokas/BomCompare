using Core;
using Core.ExcelHandling.Npoi;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CLI.Commands;

public class CompareBom : Command<CompareBomComandSettings>
{
    public override int Execute(CommandContext context, CompareBomComandSettings settings)
    {
        _ = settings.Source ?? throw new ArgumentException($"{nameof(settings.Source)} cannot be null or empty");
        _ = settings.Target ?? throw new ArgumentException($"{nameof(settings.Target)} cannot be null or empty");
        _ = settings.Output ?? throw new ArgumentException($"{nameof(settings.Output)} cannot be null or empty");
        
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