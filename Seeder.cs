using System;

namespace WeatherApi;

public class Seeder
{
    public static void SeedData(WeatherDbContext db)
    {
        if (!db.WeatherForecasts.Any())
        {
            db.WeatherForecasts.AddRange(new[]
            {
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow), TemperatureC = 25, Summary = "Sunny" },
                new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), TemperatureC = 18, Summary = "Cloudy" },
            });

            db.SaveChanges();
        }
    }
}
