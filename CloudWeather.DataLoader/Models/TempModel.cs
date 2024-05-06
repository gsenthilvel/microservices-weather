namespace CloudWeather.DataLoader.Models
{
    public class TempModel
    {
        public decimal TempHighF { get; set; }
        public decimal TempLowF { get; set; }
        public string ZipCode { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
