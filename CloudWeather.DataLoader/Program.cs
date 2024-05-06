using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var serviceConfig = configuration.GetSection("Services");

var tempServiceConfig = serviceConfig.GetSection("Temperature");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var perciptServiceConfig = serviceConfig.GetSection("Percipitation");
var perciptServiceHost = perciptServiceConfig["Host"];
var perciptServicePort = perciptServiceConfig["Port"];

var zipCodes = new List<string>{"08540","08554" };

var tempClient = new HttpClient();
tempClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var perciptClient = new HttpClient();
perciptClient.BaseAddress = new Uri($"http://{perciptServiceHost}:{perciptServicePort}");

foreach (var zipCode in zipCodes)
{
    Console.WriteLine($"Processing zip code...{zipCode}");
    var from = DateTime.Now.AddDays(-3);
    var to = DateTime.Now;

    for (var day= from.Date; day <= to.Date; day = day.AddDays(1))
    {
        var postTemp = PostTemp(zipCode, day, tempClient);
        PostPercipt(postTemp[0], zipCode, day, perciptClient);
    }
}

void PostPercipt(int lowTemp, string zipCode, DateTime day, HttpClient perciptClient)
{
    var random = new Random();
    var isPercept = random.Next(2) < 1;
    PerciptModel percipt;
    if (isPercept)
    {
        if (lowTemp < 32)
        {
            percipt = new PerciptModel
            {
                CreatedOn = day.ToUniversalTime(),
                AmountInches = random.Next(1, 16),
                WeatherType = "snow",
                ZipCode = zipCode
            };
        }
        else
        {
            percipt = new PerciptModel
            {
                CreatedOn = day.ToUniversalTime(),
                AmountInches = random.Next(1, 16),
                WeatherType = "rain",
                ZipCode = zipCode
            };
        }
    }
    else
    {
        percipt = new PerciptModel
        {
            CreatedOn = day.ToUniversalTime(),
            AmountInches = 0,
            WeatherType = "none",
            ZipCode = zipCode
        };
    }

    var percipResponse = perciptClient.
        PostAsJsonAsync("observation", percipt).
        Result;
    if (!percipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Failed to post perciptation data for {zipCode} on {day}");
    }
    else 
    {
        Console.WriteLine($"Posted perciptation type {percipt.WeatherType} " +
            $"at zip {zipCode} for {zipCode} on {day}");
    }

}

List<int> PostTemp(string zipCode, DateTime day, HttpClient tempClient)
{
    var random = new Random();
    var tempLowHigh = new List<int> { random.Next(0, 100), random.Next(0, 100) };
    tempLowHigh.Sort();
    var tempModel = new TempModel
    {
        CreatedOn = day.ToUniversalTime(),
        TempHighF = tempLowHigh[1],
        TempLowF = tempLowHigh[0],
        ZipCode = zipCode
    };
    var tempResponse = tempClient.
        PostAsJsonAsync("observation", tempModel).
        Result;
    if (!tempResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Failed to post temperature data for {zipCode} on {day}");
    }
    else
    {
        Console.WriteLine($"Posted temperature high {tempModel.TempHighF} " +
                       $"and low {tempModel.TempLowF} at zip {zipCode} for {zipCode} on {day}");
    }
    return tempLowHigh;
}