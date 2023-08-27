using Spectre.Console.Cli;

namespace CLI.Commands;

public class CompareBomComandSettings : CommandSettings
{
    [CommandOption("-s|--source <xlsx>")]
    public string Source { get; set; }
    
    [CommandOption("-t|--target <xlsx>")]
    public string Target { get; set; }
    
    [CommandOption("-o|--output <xlsx>")]
    public string Output { get; set; }
}