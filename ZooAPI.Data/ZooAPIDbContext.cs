using Microsoft.EntityFrameworkCore;
using ZooAPI.Data.ModelConfigs;
using ZooAPI.Domain.Models;

namespace ZooAPI.Data
{
    public class ZooAPIDbContext : DbContext
    {
        private readonly IAnimalConfig _animalConfig;

        public ZooAPIDbContext(DbContextOptions<ZooAPIDbContext> options, IAnimalConfig animalConfig) : base(options)
        {
            _animalConfig = animalConfig;
        }

        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _animalConfig.Configure(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
