using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAccess
{
    public class TempDbContext : DbContext
    {
        public TempDbContext() { }
        public TempDbContext(DbContextOptions<TempDbContext> options) : base(options) { }
        public DbSet<Temperature> Temperatures { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }
        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(
                               blob =>
                               {
                                   blob.ToTable("temperature");
                               });
        }
    }
}
