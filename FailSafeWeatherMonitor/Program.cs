using FailSafeWeatherMonitor.Services;
using Spectre.Console;

while (true)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new Rule("[bold blue]FailSafe WeatherMonitor[/]"));

    string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Choose an option:")
            .AddChoices(
                "Import historical data from file",
                "Fetch live data from API",
                "Show weather statistics",
                "Exit"));

    if (choice == "Exit")
        break;

    switch (choice)
    {
        case "Import historical data from file":
            FileImportService.ImportFromFile();
            break;
        case "Fetch live data from API":
            AnsiConsole.MarkupLine("[yellow]Coming soon...[/]");
            break;
        case "Show weather statistics":
            AnsiConsole.MarkupLine("[yellow]Coming soon...[/]");
            break;
    }

    AnsiConsole.MarkupLine("\n[grey]Press any key to go back to the menu...[/]");
    Console.ReadKey(true);
}