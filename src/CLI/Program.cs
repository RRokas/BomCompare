using CLI;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<CompareBom>("compare")
        .WithDescription("Compares two BOM Excel files and outputs the comparison to a new xlsx file.")
        .WithExample("compare -s source.xlsx -t target.xlsx -o output.xlsx");
});

return app.Run(args);