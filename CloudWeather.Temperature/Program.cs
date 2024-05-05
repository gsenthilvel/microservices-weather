using CloudWeather.Temperature.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TempDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip, [FromQuery] int? days, TempDbContext db) =>
{
    if (days == null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide a number of days between 1 and 30.");
    }
    var startData = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Temperatures.Where(
               p => p.ZipCode == zip && p.CreatedOn >= startData
                      ).ToListAsync();
    return Results.Ok(results);
});

app.MapPost("/observation", async (Temperature observation, TempDbContext db) =>
{
    db.Temperatures.Add(observation);
    await db.SaveChangesAsync();
    return Results.Created($"/observation/{observation.Id}", observation);
});

app.Run();
