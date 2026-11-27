using System.Globalization;
using System.Net.Http.Json;
using FailSafeWeatherMonitor.Models;
using Spectre.Console;

namespace FailSafeWeatherMonitor.Services;

public static class ApiService
{
    private const string ApiUrl =
        "https://api.open-meteo.com/v1/forecast?latitude=57.6832&longitude=11.9511&hourly=temperature_2m,relative_humidity_2m";

    private const string StationName = "Goteborg_OpenMeteo";

    public static void FetchLiveData()
    {
        try
        {
            using var http = new HttpClient();

            OpenMeteoResponse? response = http.GetFromJsonAsync<OpenMeteoResponse>(ApiUrl).Result;

            if (response is null)
            {
                AnsiConsole.MarkupLine("[red]The API gave an empty answer.[/]");
                return;
            }

            string currentHour = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:00", CultureInfo.InvariantCulture);
            int index = response.Hourly.Time.IndexOf(currentHour);

            if (index == -1)
            {
                AnsiConsole.MarkupLine("[red]Could not find the current hour in the API answer.[/]");
                return;
            }

            var point = new WeatherDataPoint
            {
                StationName = StationName,
                Timestamp = DateTime.Parse(currentHour, CultureInfo.InvariantCulture),
                Temperature = response.Hourly.Temperature[index],
                Humidity = response.Hourly.Humidity[index],
                DataSource = "API"
            };

            ShowPoint(point);

            int savedCount = DatabaseService.SaveNewPoints(new List<WeatherDataPoint> { point });

            if (savedCount == 1)
                AnsiConsole.MarkupLine("[green]The measurement was saved to the database.[/]");
            else
                AnsiConsole.MarkupLine("[yellow]This hour is already saved. Nothing new was saved.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Could not fetch live data. Check your internet connection.[/]");
            AnsiConsole.WriteLine(ex.Message);
        }
    }

    private static void ShowPoint(WeatherDataPoint point)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold]Live weather from Open-Meteo[/]");

        table.AddColumn("[blue]Station[/]");
        table.AddColumn("[blue]Time (UTC)[/]");
        table.AddColumn("[blue]Temperature (°C)[/]");
        table.AddColumn("[blue]Humidity (%)[/]");

        table.AddRow(
            point.StationName,
            point.Timestamp.ToString("yyyy-MM-dd HH:mm"),
            point.Temperature.ToString("0.0"),
            point.Humidity.ToString("0.0"));

        AnsiConsole.Write(table);
    }
}