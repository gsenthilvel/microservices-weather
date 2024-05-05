using CloudWeather.Report.DataAccess;

namespace CloudWeather.Report.BusinessLogic
{
    public interface IWeatherReportAggregator
    {
        public Task<WeatherReport> GetWeatherReportsAsync(string zip, int days);
    }
}
