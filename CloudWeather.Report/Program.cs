using CloudWeather.Report.BusinessLogic;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

builder.Services.AddDbContext<WeatherReportDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);


var app = builder.Build();
app.MapGet(
    "/weather-report/{zip}", 
    async (string zip, [FromQuery] int? days, IWeatherReportAggregator weatherReportAggregator) =>
{
    if (days == null || days > 30 || days < 1)
    {
        return Results.BadRequest("Please provide the number of days to get the weather report for.");
    }
    var report = await weatherReportAggregator.GetWeatherReportsAsync(zip, days.Value);
    return Results.Ok(report);
});

app.Run();
