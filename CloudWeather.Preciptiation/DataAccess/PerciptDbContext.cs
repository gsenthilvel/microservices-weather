using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Preciptiation.DataAccess
{
    public class PerciptDbContext : DbContext
    {
        public PerciptDbContext() { }
        public PerciptDbContext(DbContextOptions<PerciptDbContext> options) : base(options) { }
        public DbSet<Perciptiation> Perciptiations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTableNames(modelBuilder);
        }

        private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Perciptiation>(
                blob => { 
                    blob.ToTable("precipitation"); 
                });
        }
    }
}
