using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;
using System.Text.Json;

namespace CloudWeather.Report.BusinessLogic
{
    public class WeatherReportAggregator : IWeatherReportAggregator
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WeatherReportAggregator> _logger;
        private readonly WeatherDataConfig _config;
        private readonly WeatherReportDbContext _db;
        public WeatherReportAggregator(IHttpClientFactory clientFactory, ILogger<WeatherReportAggregator> logger, WeatherDataConfig config, WeatherReportDbContext db)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _config = config;
            _db = db;
        }

        public async Task<WeatherReport> GetWeatherReportsAsync(string zip, int days)
        {
            var httpClient = _clientFactory.CreateClient();

            var perciptData = await GetPerciptData(httpClient, zip, days);
            var totalSnow = perciptData.Where(p => p.WeatherType == "Snow").Sum(p => p.AmountInches);
            var totalRain = perciptData.Where(p => p.WeatherType == "Rain").Sum(p => p.AmountInches);

            var tempData = await GetTempData(httpClient, zip, days);
            var averageHighTemp = tempData.Average(t => t.TempHighF);
            var averageLowTemp = tempData.Average(t => t.TempLowF);

            _logger.LogInformation($"Weather report for {zip} for the next {days} days: ");
            _logger.LogInformation($"Total snow: {totalSnow} inches");
            _logger.LogInformation($"Total rain: {totalRain} inches");
            _logger.LogInformation($"Average high temp: {averageHighTemp} F");
            _logger.LogInformation($"Average low temp: {averageLowTemp} F");

            var weatherReport = new WeatherReport
            {
                ZipCode = zip,
                SnowfallInches = totalSnow,
                RailfallInches = totalRain,
                AverageHighF = averageHighTemp,
                AverageLowF = averageLowTemp,
                CreatedOn = DateTime.UtcNow
            };

            _db.WeatherReports.Add(weatherReport);
            await _db.SaveChangesAsync();

            return weatherReport;
        }

        private async Task<List<TempModel>> GetTempData(HttpClient httpClient, string zip, int days)
        {
            var endPoint = BuildTempServiceEndPoint(zip, days);
            var response = await httpClient.GetAsync(endPoint);
            var tempData = await response.Content.
                ReadFromJsonAsync<List<TempModel>>();
            return tempData ?? new List<TempModel>();
        }

        private string? BuildTempServiceEndPoint(string zip, int days)
        {
            var serviceProtocol = _config.TempDataProtocol;
            var serviceHost = _config.TempDataHost;
            var servicePort = _config.TempDataPort;

            return $"{serviceProtocol}://{serviceHost}:{servicePort}/observation/{zip}?days={days}";
        }

        private async Task<List<PerciptModel>> GetPerciptData(HttpClient httpClient, string zip, int days)
        {
            var endPoint = BuildPerciptServiceEndPoint(zip, days);
            var response = await httpClient.GetAsync(endPoint);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var perciptData = await response.Content.
                ReadFromJsonAsync<List<PerciptModel>>(jsonSerializerOptions);
            return perciptData ?? new List<PerciptModel>();
        }

        private string? BuildPerciptServiceEndPoint(string zip, int days)
        {
            var serviceProtocol = _config.PerciptDataProtocol;
            var serviceHost = _config.PerciptDataHost;
            var servicePort = _config.PerciptDataPort;

            return $"{serviceProtocol}://{serviceHost}:{servicePort}/observation/{zip}?days={days}";
        }
    }
}
