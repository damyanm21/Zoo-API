
using ZooAPI.Domain.Models;
using ZooAPI.Domain.Models.DTOs;

namespace ZooAPI.BusinessLogic
{
    public interface IAnimalService
    {
        Task<List<AnimalDto>> GetSortedAnimalsAsync();
        Task<Animal> GetAnimalByIdAsync(int id);
        Task<int> AddAnimalAsync(AnimalDto animal);
        Task<bool> UpdateAnimalAsync(int id, string name, string species, AnimalClass animalClass, AnimalType animalType, decimal weight);
        Task<bool> DeleteAnimalAsync(int id);
    }
}
