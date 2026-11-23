using FailSafeWeatherMonitor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FailSafeWeatherMonitor.Data;

public class WeatherMonitorDbContext : DbContext
{
    public DbSet<WeatherDataPoint> WeatherDataPoints { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Read the connection string from appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder.UseSqlServer(config.GetConnectionString("WeatherDb"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UNIQUE index: the same station can't have two measurements with the same timestamp
        modelBuilder.Entity<WeatherDataPoint>()
            .HasIndex(w => new { w.StationName, w.Timestamp })
            .IsUnique();
    }
}