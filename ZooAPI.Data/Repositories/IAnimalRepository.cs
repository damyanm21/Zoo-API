using ZooAPI.Domain.Models;

namespace ZooAPI.Data.Repositories
{
    public interface IAnimalRepository
    {
        Task<List<Animal>> GetAnimalsAsync();
        Task<Animal> GetAnimalByIdAsync(int id);
        Task<int> AddAnimalAsync(Animal animal);
        Task<bool> UpdateAnimalAsync(int id, string name, string species, AnimalClass animalClass, AnimalType animalType, decimal weight);
        Task<bool> DeleteAnimalAsync(int id);
    }
}
