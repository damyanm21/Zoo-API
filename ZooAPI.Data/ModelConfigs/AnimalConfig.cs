using Microsoft.EntityFrameworkCore;
using ZooAPI.Domain.Models;

namespace ZooAPI.Data.ModelConfigs
{
    public class AnimalConfig : IAnimalConfig
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Species).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Class).IsRequired();
                entity.Property(e => e.Weight).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.FoodNeeded).HasPrecision(18, 2);
            });
        }
    }
}
