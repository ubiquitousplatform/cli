// See https://aka.ms/new-console-template for more information


// TODO: fancify with Terminal.Gui

using System.Net.Http.Handlers;
using System.Xml.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
// Create a command using Spectre.Console.Cli for downloading a file from a URL
//


public class DownloadTemplateCommand : AsyncCommand<DownloadTemplateCommand.Settings>
{


    private static readonly List<string> allowedTemplates = new() { "javascript", "typescript" };
    public class Settings : CommandSettings
    {

        public override ValidationResult Validate()
        {
            return allowedTemplates.Contains(Template)
                ? ValidationResult.Success()
                : ValidationResult.Error($"Unable to initialize project. Template '{Template}' not found. Supported templates: {string.Join(",", allowedTemplates)}");
        }

        [CommandArgument(0, "<template>")]
        public string Template { get; set; }

        [CommandArgument(1, "<project-name>")]
        public string ProjectName { get; set; }
    }


    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // Use System.Runtime.InteropServices.RuntimeInformation.OSArchitecture to get the OS architecture
        // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.runtimeinformation.osarchitecture?view=net-5.0
        AnsiConsole.WriteLine($"OS Architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");

        var template = settings.Template;
        var output = settings.ProjectName ?? $"{template}.zip";
        var url = $"https://github.com/username/{template}/releases/latest/download/{template}.zip";

        AnsiConsole.WriteLine($"Downloading {template} template...");
        AnsiConsole.WriteLine($"From: {url}");
        AnsiConsole.WriteLine($"To: {output}");
        var handler = new HttpClientHandler() { AllowAutoRedirect = true };
        var ph = new ProgressMessageHandler(handler);

        await AnsiConsole.Progress()
               .Columns(new ProgressColumn[]
               {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),       
                    new RemainingTimeColumn(),      // Remaining time
                    new SpinnerColumn(),            // Spinner
               }).StartAsync(async ctx =>
               {
                   var task1 = ctx.AddTask($"Downloading {template} template...", new ProgressTaskSettings { MaxValue = 100 });

                   task1.StartTask();
                   /* Download the template and update progress as it downloads */
                   
                   ph.HttpReceiveProgress += (_, args) =>
                   {
                       // calculate the progress percentage and update the task
                       task1.Value = args.ProgressPercentage;
                   };

                   var client = new HttpClient(ph);
                   client.Timeout = TimeSpan.FromSeconds(240);
                   var response = await client.GetAsync(url);
                   // check to make sure we got a successful response and didn't time out
                   response.EnsureSuccessStatusCode();
                   // read the response and write it to a file
                   await response.Content.ReadAsStreamAsync();

                   var task2 = ctx.AddTask($"Extracting {template} template...", new ProgressTaskSettings { MaxValue = 100 });
               });


        //ph.HttpReceiveProgress += (sender, args) =>
        //{
        //    progressBar...StartAsync(async ctx =>
        //        {
        //            await ctx.
        //                SetValue(args.ProgressPercentage)
        //                .Task($"Downloading {template} template...")
        //                .MaxValue(100);
        //        });
        //        .Start(ctx =>
        //        {
        //            ctx.
        //                .SetValue(args.ProgressPercentage)
        //                .Task($"Downloading {template} template...")
        //                .MaxValue(100);
        //        });
        //};
        //using (var client = new HttpClient())
        //{
        //    /*
        //    client.DownloadProgressChanged += (sender, e) =>
        //    {
        //        AnsiConsole.Progress()
        //            .Columns(new ProgressColumn[]
        //            {
        //                new TaskDescriptionColumn(),
        //                new ProgressBarColumn(),
        //                new PercentageColumn(),
        //            })
        //            .Start(ctx =>
        //            {
        //                ctx.Progress
        //                    .SetValue(e.ProgressPercentage)
        //                    .Task($"Downloading {template} template...")
        //                    .MaxValue(100);
        //            });
        //    };*/

        //}

        AnsiConsole.MarkupLine($"[green]Downloaded {template} template to {output}[/]");

        return 0;
    }
}

