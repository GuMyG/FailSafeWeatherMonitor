using FailSafeWeatherMonitor.Data;
using FailSafeWeatherMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace FailSafeWeatherMonitor.Services;

public static class DatabaseService
{
    public static int SaveNewPoints(List<WeatherDataPoint> points)
    {
        using var _db = new WeatherMonitorDbContext();
        int savedCount = 0;

        foreach (WeatherDataPoint point in points)
        {
            bool exists = _db.WeatherDataPoints.Any(w =>
                w.StationName == point.StationName && w.Timestamp == point.Timestamp);

            if (exists)
                continue;

            _db.WeatherDataPoints.Add(point);
            _db.SaveChanges();
            savedCount++;
        }

        return savedCount;
    }

    public static List<string> GetStationNames()
    {
        using var _db = new WeatherMonitorDbContext();

        return _db.WeatherDataPoints
            .Select(w => w.StationName)
            .Distinct()
            .OrderBy(name => name)
            .ToList();
    }

    public static List<WeatherDataPoint> GetMeasurements(string stationName)
    {
        using var _db = new WeatherMonitorDbContext();

        return _db.WeatherDataPoints
            .AsNoTracking()
            .Where(w => w.StationName == stationName)
            .OrderBy(w => w.Timestamp)
            .ToList();
    }
}