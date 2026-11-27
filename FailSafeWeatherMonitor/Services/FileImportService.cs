using System.Globalization;
using FailSafeWeatherMonitor.Models;
using Spectre.Console;

namespace FailSafeWeatherMonitor.Services;
public static class FileImportService
{
    private const string DataFile = "weatherdata.csv";
    private const string LogFile = "bad_data.log";

    public static void ImportFromFile()
    {
        try
        {
            string[] lines = File.ReadAllLines(DataFile);

            List<WeatherDataPoint> validPoints = new();
            List<string> badLines = new();

            foreach (string line in lines.Skip(1))
            {
                WeatherDataPoint? point = ParseLine(line);

                if (point is null)
                    badLines.Add(line);
                else
                    validPoints.Add(point);
            }

            File.WriteAllLines(LogFile, badLines);

            int savedCount = DatabaseService.SaveNewPoints(validPoints);
            int duplicateCount = validPoints.Count - savedCount;

            AnsiConsole.MarkupLine($"[green]{savedCount} rows imported[/], [red]{badLines.Count} rows rejected because of errors[/].");
            AnsiConsole.MarkupLine($"[yellow]{duplicateCount} duplicates skipped.[/]");
            AnsiConsole.MarkupLine($"[grey]The rejected rows were written to {LogFile}.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]The import failed:[/]");
            AnsiConsole.WriteLine(ex.Message);
        }
    }

    private static WeatherDataPoint? ParseLine(string line)
    {
        string[] parts = line.Split('\t');

        if (parts.Length != 4)
            return null;

        string stationName = parts[0].Trim();
        if (stationName == "")
            return null;

        if (!DateTime.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
            return null;

        if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double temperature))
            return null;

        if (!double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double humidity))
            return null;

        if (temperature < -50 || temperature > 50)
            return null;

        if (humidity < 0 || humidity > 100)
            return null;

        return new WeatherDataPoint
        {
            StationName = stationName,
            Timestamp = timestamp,
            Temperature = temperature,
            Humidity = humidity,
            DataSource = "CSV-File"
        };
    }
}