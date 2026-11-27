using System.Text.Json.Serialization;

namespace FailSafeWeatherMonitor.Models;
public class OpenMeteoResponse
{
    [JsonPropertyName("hourly")]
    public HourlyData Hourly { get; set; } = null!;
}
public class HourlyData
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature { get; set; } = new();

    [JsonPropertyName("relative_humidity_2m")]
    public List<double> Humidity { get; set; } = new();
}