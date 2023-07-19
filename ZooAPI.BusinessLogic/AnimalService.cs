using ZooAPI.BusinessLogic;
using ZooAPI.Data.Repositories;
using ZooAPI.Domain.Models;
using ZooAPI;
using ZooAPI.Domain.Models.DTOs;
using AutoMapper;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _animalRepository;
    private readonly IMapper _mapper;

    public AnimalService(IAnimalRepository animalRepository, IMapper mapper)
    {
        _animalRepository = animalRepository;
        _mapper = mapper;
    }

    public async Task<List<AnimalDto>> GetSortedAnimalsAsync()
    {
        var animals = await _animalRepository.GetAnimalsAsync();

        var sortedAnimals = animals
            .OrderBy(a => a.Type != AnimalType.Herbivore && a.Type != AnimalType.Carnivore) // Separate herbivores and carnivores from other animals
            .ThenByDescending(a => a.Class == AnimalClass.Bird && a.Weight > animals.Where(an => an.Class == AnimalClass.Reptile).Max(an => an.Weight)) // Sort birds and reptiles based on the third rule
            .ThenBy(a => a.Class) // Sort animals within the same class
            .ThenBy(a => a.Weight) // Sort animals by weight
            .ToList();

        var animalDtos = _mapper.Map<List<AnimalDto>>(sortedAnimals);
        return animalDtos;
    }


    public async Task<Animal> GetAnimalByIdAsync(int id)
    {
        var animal = await _animalRepository.GetAnimalByIdAsync(id);
        if (animal == null)
            return null;

        decimal foodNeeded;
        if (animal.Type == AnimalType.Carnivore)
        {
            var heaviestHerbivoreWeight = (await _animalRepository.GetAnimalsAsync())
                .Where(a => a.Type == AnimalType.Herbivore)
                .Max(a => a.Weight);
            foodNeeded = heaviestHerbivoreWeight + animal.Weight * 0.25m;
        }
        else if (animal.Type == AnimalType.Herbivore)
        {
            var averageHerbivoreWeight = (await _animalRepository.GetAnimalsAsync())
                .Where(a => a.Type == AnimalType.Herbivore)
                .Average(a => a.Weight);
            foodNeeded = averageHerbivoreWeight + animal.Weight * 0.25m;
        }
        else // AnimalType.Omnivore
        {
            var heaviestHerbivoreWeight = (await _animalRepository.GetAnimalsAsync())
                .Where(a => a.Type == AnimalType.Herbivore)
                .Max(a => a.Weight);
            var averageHerbivoreWeight = (await _animalRepository.GetAnimalsAsync())
                .Where(a => a.Type == AnimalType.Herbivore)
                .Average(a => a.Weight);
            foodNeeded = (heaviestHerbivoreWeight + animal.Weight * 0.25m) * 0.5m + (averageHerbivoreWeight + animal.Weight * 0.25m) * 0.5m;
        }

        animal.FoodNeeded = foodNeeded;
        return animal;
    }

    public async Task<int> AddAnimalAsync(AnimalDto animalDto)
    {
        // Perform validation
        if (string.IsNullOrEmpty(animalDto.Name) || animalDto.Name.Length > 255)
            throw new ArgumentException(Consts.ErrorNameLength);

        var animals = await _animalRepository.GetAnimalsAsync();
        if (animals.Any(a => a.Name == animalDto.Name))
            throw new ArgumentException(Consts.ErrorNameUnique);

        if (string.IsNullOrEmpty(animalDto.Species) || animalDto.Species.Length > 255)
            throw new ArgumentException(Consts.ErrorSpeciesLength);

        if (animalDto.Weight <= 0)
            throw new ArgumentException(Consts.ErrorWeightNumber);

        if (!Enum.IsDefined(typeof(AnimalClass), animalDto.Class))
            throw new ArgumentException();

        if (!Enum.IsDefined(typeof(AnimalType), animalDto.Type))
            throw new ArgumentException(Consts.ErrorValidType);

        var animal = _mapper.Map<Animal>(animalDto);
        return await _animalRepository.AddAnimalAsync(animal);
    }


    public async Task<bool> UpdateAnimalAsync(int id, string name, string species, AnimalClass animalClass, AnimalType animalType, decimal weight)
    {
        // Perform validation
        if (string.IsNullOrEmpty(name) || name.Length > 255)
            throw new ArgumentException(Consts.ErrorNameLength);

        var animals = await _animalRepository.GetAnimalsAsync();
        if (animals.Any(a => a.Name == name && a.Id != id))
            throw new ArgumentException(Consts.ErrorNameUnique);

        if (weight <= 0)
            throw new ArgumentException(Consts.ErrorWeightNumber);

        return await _animalRepository.UpdateAnimalAsync(id, name, species, animalClass, animalType, weight);
    }

    public async Task<bool> DeleteAnimalAsync(int id)
    {
        return await _animalRepository.DeleteAnimalAsync(id);
    }
}
