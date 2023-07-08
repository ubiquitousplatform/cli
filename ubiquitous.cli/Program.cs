// See https://aka.ms/new-console-template for more information


// TODO: fancify with Terminal.Gui

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
/*
var isRegistered = false;

if (!isRegistered)
{
    Console.WriteLine("Welcome to the Ubiquitous Platform!");
    Console.WriteLine("Please register.");
}
*/

AnsiConsole.Markup("[underline red]Hello[/] World!");
// Create a command using Spectre.Console.Cli for downloading a file from a URL
// https://spectresystems.github.io/spectre.console/articles/cli.html
//


public class DownloadTemplateCommand : AsyncCommand<DownloadTemplateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<template>")]
        public string Template { get; set; }

        [CommandOption("-o|--output <output>")]
        public string Output { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Use System.Runtime.InteropServices.RuntimeInformation.OSArchitecture to get the OS architecture
        // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.runtimeinformation.osarchitecture?view=net-5.0
        AnsiConsole.WriteLine($"OS Architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");

        var template = settings.Template;
        var output = settings.Output ?? $"{template}.zip";
        var url = $"https://github.com/username/{template}/releases/latest/download/{template}.zip";

        AnsiConsole.WriteLine($"Downloading {template} template...");
        AnsiConsole.WriteLine($"From: {url}");
        AnsiConsole.WriteLine($"To: {output}");

        using (var client = new WebClient())
        {
            client.DownloadProgressChanged += (sender, e) =>
            {
                AnsiConsole.Progress()
                    .Columns(new ProgressColumn[]
                    {
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                    })
                    .Start(ctx =>
                    {
                        ctx.Progress
                            .SetValue(e.ProgressPercentage)
                            .Task($"Downloading {template} template...")
                            .MaxValue(100);
                    });
            };

            await client.DownloadFileTaskAsync(new Uri(url), output);
        }

        AnsiConsole.MarkupLine($"[green]Downloaded {template} template to {output}[/]");

        return 0;
    }
}

