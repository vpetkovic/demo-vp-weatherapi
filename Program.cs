using Microsoft.EntityFrameworkCore;
using WeatherApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WeatherApiDatabase");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<WeatherDbContext>(options => options.UseInMemoryDatabase("WeatherDb"));
    Console.WriteLine("No connection string provided. Falling back to In-Memory Database");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
    try
    {
        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
        else {
            dbContext.Database.EnsureCreated();
        }
        Seeder.SeedData(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating and seeding database: {ex.Message}");
    }
}

app.MapGet("/weather", async (WeatherDbContext db) =>
{
    var forecasts = await db.WeatherForecasts.ToListAsync();
    return forecasts.Any() ? Results.Ok(forecasts) : Results.NotFound("No data available");
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();