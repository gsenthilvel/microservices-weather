namespace CloudWeather.Preciptiation.DataAccess
{
    public class Perciptiation
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal AmountInches { get; set; }
        public string WeatherType { get; set; }
        public string ZipCode { get; set; }

    }
}
