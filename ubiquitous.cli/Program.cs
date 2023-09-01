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

// AnsiConsole.Markup("[underline red]Hello[/] World!");

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("ubiq");
    config.SetApplicationVersion("0.1.0");
    config.AddCommand<DownloadTemplateCommand>("init")
        .WithAlias("initialize")
        .WithDescription("Initialize a new project under the specified folder")
        .WithExample("init", "javascript", "my-project-name");
        //.WithExample("typescript", "my-project-name");
});

return app.Run(args);