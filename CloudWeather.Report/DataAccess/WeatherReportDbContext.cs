using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Report.DataAccess
{
    public class WeatherReportDbContext:DbContext
    {
        public WeatherReportDbContext() { }
        public WeatherReportDbContext(DbContextOptions<WeatherReportDbContext> options) : base(options) { }
        public DbSet<WeatherReport> WeatherReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }
        public static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>(
                               blob =>
                               {
                                   blob.ToTable("weather_report");
                               });
        }
    }
}
