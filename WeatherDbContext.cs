using System;
using Microsoft.EntityFrameworkCore;

namespace WeatherApi;

// Data Model & DB Context
public class WeatherForecast
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
    public DbSet<WeatherForecast> WeatherForecasts { get; set ;}
}

