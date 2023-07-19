using Microsoft.EntityFrameworkCore;
using ZooAPI.Domain.Models;

namespace ZooAPI.Data.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ZooAPIDbContext _dbContext;

        public AnimalRepository(ZooAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Animal>> GetAnimalsAsync()
        {
            return await _dbContext.Animals.ToListAsync();
        }

        public async Task<Animal> GetAnimalByIdAsync(int id)
        {
            return await _dbContext.Animals.FindAsync(id);
        }

        public async Task<int> AddAnimalAsync(Animal animal)
        {
            _dbContext.Animals.Add(animal);
            await _dbContext.SaveChangesAsync();
            return animal.Id;
        }

        public async Task<bool> UpdateAnimalAsync(int id, string name, string species, AnimalClass animalClass, AnimalType animalType, decimal weight)
        {
            var animal = await _dbContext.Animals.FindAsync(id);
            if (animal == null)
                return false;

            animal.Name = name;
            animal.Species = species;
            animal.Class = animalClass;
            animal.Type = animalType;
            animal.Weight = weight;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnimalAsync(int id)
        {
            var animal = await _dbContext.Animals.FindAsync(id);
            if (animal == null)
                return false;

            _dbContext.Animals.Remove(animal);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
