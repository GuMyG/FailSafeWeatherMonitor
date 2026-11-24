using FailSafeWeatherMonitor.Data;
using FailSafeWeatherMonitor.Models;

namespace FailSafeWeatherMonitor.Services;

// All work against the database is done in this class
public static class DatabaseService
{
    // Saves the points that are not already in the database.
    // Returns how many points were saved.
    public static int SaveNewPoints(List<WeatherDataPoint> points)
    {
        using var _db = new WeatherMonitorDbContext();
        int savedCount = 0;

        foreach (WeatherDataPoint point in points)
        {
            // Duplicate check: same station and same timestamp already saved?
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
}