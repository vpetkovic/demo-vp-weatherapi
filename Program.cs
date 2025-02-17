using Microsoft.EntityFrameworkCore;
using WeatherApi;

var builder = WebApplication.CreateBuilder(args);

// Get DB Connection String from ENV Variable
var connectionString = builder.Configuration.GetConnectionString("WeatherApiDatabase") 
    ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    
}

if (!string.IsNullOrEmpty(connectionString))
{
    // Use MSSQL if connection string is provided
    builder.Services.AddDbContext<WeatherDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    // Use In-Memory Database as fallback
    builder.Services.AddDbContext<WeatherDbContext>(options => options.UseInMemoryDatabase("WeatherDb"));
    Console.WriteLine("No connection string provided. Falling back to In-Memory Database");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure DB is created & apply migrations
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

// Weather Forecast API
app.MapGet("/weather", async (WeatherDbContext db) =>
{
    var forecasts = await db.WeatherForecasts.ToListAsync();
    return forecasts.Any() ? Results.Ok(forecasts) : Results.NotFound("No data available");
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();