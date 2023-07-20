using Moq;
using ZooAPI.Data.Repositories;
using ZooAPI.Domain.Models;
using ZooAPI.Domain.Models.DTOs;
using AutoMapper;
using ZooAPI.Domain.Models.Profiles;
using ZooAPI;

namespace Tests;

[TestFixture]
public class AnimalServiceTests
{
    private Mock<IAnimalRepository> _animalRepositoryMock;
    private IMapper _mapper;
    private AnimalService _animalService;
    private List<Animal> _animals;

    [SetUp]
    public void Setup()
    {
        _animalRepositoryMock = new Mock<IAnimalRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AnimalMappingProfile>());
        _mapper = config.CreateMapper();
        _animalService = new AnimalService(_animalRepositoryMock.Object, _mapper);

        _animals = new List<Animal>
        {
            new Animal { Id = 1, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 50 },
            new Animal { Id = 2, Type = AnimalType.Carnivore, Class = AnimalClass.Bird, Weight = 30 },
            new Animal { Id = 3, Type = AnimalType.Omnivore, Class = AnimalClass.Mammal, Weight = 40 },
            new Animal { Id = 4, Type = AnimalType.Herbivore, Class = AnimalClass.Reptile, Weight = 60 },
            new Animal { Id = 5, Type = AnimalType.Carnivore, Class = AnimalClass.Reptile, Weight = 70 },
            new Animal { Id = 6, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 90 },
            new Animal { Id = 7, Type = AnimalType.Herbivore, Class = AnimalClass.Bird, Weight = 10 },
            new Animal { Id = 8, Type = AnimalType.Omnivore, Class = AnimalClass.Bird, Weight = 20 },
        };
    }

    [Test]
    public async Task GetSortedAnimalsAsync_ShouldReturnSortedAnimals()
    {
        // Arrange
        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(_animals);

        // Act
        var result = await _animalService.GetSortedAnimalsAsync();

        // Assert
        var expectedSortedAnimals = _animals
            .OrderBy(a => a.Type != AnimalType.Herbivore && a.Type != AnimalType.Carnivore)
            .ThenByDescending(a => a.Class == AnimalClass.Bird && a.Weight > _animals.Where(an => an.Class == AnimalClass.Reptile).Max(an => an.Weight))
            .ThenBy(a => a.Class)
            .ThenBy(a => a.Weight)
            .ToList();

        var expectedAnimalDtos = _mapper.Map<List<AnimalDto>>(expectedSortedAnimals);

        Assert.AreEqual(expectedAnimalDtos.Count, result.Count);
        for (int i = 0; i < expectedAnimalDtos.Count; i++)
        {
            Assert.AreEqual(expectedAnimalDtos[i].Id, result[i].Id);
            Assert.AreEqual(expectedAnimalDtos[i].Type, result[i].Type);
            Assert.AreEqual(expectedAnimalDtos[i].Class, result[i].Class);
            Assert.AreEqual(expectedAnimalDtos[i].Weight, result[i].Weight);
        }
    }

    [Test]
    public async Task GetAnimalByIdAsync_ExistingId_ShouldReturnCorrectAnimal()
    {
        // Arrange
        var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 },
            new Animal { Id = 2, Name = Consts.Elephant, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 5000 }
        };

        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(animals);
        _animalRepositoryMock.Setup(repo => repo.GetAnimalByIdAsync(1)).ReturnsAsync(animals[0]);
        var animalService = new AnimalService(_animalRepositoryMock.Object, _mapper);

        // Act
        var result = await animalService.GetAnimalByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual(Consts.Lion, result.Name);
        Assert.AreEqual(5050m, result.FoodNeeded);
    }

    [Test]
    public async Task GetAnimalByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        _animalRepositoryMock.Setup(repo => repo.GetAnimalByIdAsync(99)).ReturnsAsync((Animal)null);
        var animalService = new AnimalService(_animalRepositoryMock.Object, _mapper);

        // Act
        var result = await animalService.GetAnimalByIdAsync(99);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task AddAnimalAsync_ValidInput_ShouldReturnNewAnimalId()
    {
        // Arrange
        var newAnimalDto = new AnimalDto { Name = Consts.Giraffe, Species = Consts.GiraffaCamelopardalis, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 1200 };

        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(_animals);
        _animalRepositoryMock.Setup(repo => repo.AddAnimalAsync(It.IsAny<Animal>())).ReturnsAsync(3); // Simulate adding a new animal with Id 3

        // Act
        var result = await _animalService.AddAnimalAsync(newAnimalDto);

        // Assert
        Assert.AreEqual(3, result);
    }

    [Test]
    public async Task AddAnimalAsync_DuplicateName_ShouldThrowArgumentException()
    {
        // Arrange
        var newAnimalDto = new AnimalDto { Name = Consts.Lion, Species = Consts.PantheraLeo, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 180 };

        var existingAnimals = new List<Animal>
        {
            new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 },
            new Animal { Id = 2, Name = Consts.Elephant, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 5000 }
        };

        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(existingAnimals);
        var animalService = new AnimalService(_animalRepositoryMock.Object, _mapper);

        // Act and Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await animalService.AddAnimalAsync(newAnimalDto));
    }

    [Test]
    public async Task UpdateAnimalAsync_ValidInput_ShouldReturnTrue()
    {
        // Arrange
        var existingAnimal = new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 };
        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(new List<Animal> { existingAnimal });
        _animalRepositoryMock.Setup(repo => repo.UpdateAnimalAsync(1, Consts.Lioness, Consts.PantheraLeo, AnimalClass.Mammal, AnimalType.Carnivore, 180)).ReturnsAsync(true);

        // Act
        var result = await _animalService.UpdateAnimalAsync(1, Consts.Lioness, Consts.PantheraLeo, AnimalClass.Mammal, AnimalType.Carnivore, 180);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task UpdateAnimalAsync_DuplicateName_ShouldThrowArgumentException()
    {
        // Arrange
        var existingAnimals = new List<Animal>
    {
        new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 },
        new Animal { Id = 2, Name = Consts.Elephant, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 5000 }
    };

        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(existingAnimals);
        _animalRepositoryMock.Setup(repo => repo.UpdateAnimalAsync(1, Consts.Elephant, Consts.LoxodontaAfricana, AnimalClass.Mammal, AnimalType.Herbivore, 6000)).ReturnsAsync(false);

        // Act and Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _animalService.UpdateAnimalAsync(1, Consts.Elephant, Consts.LoxodontaAfricana, AnimalClass.Mammal, AnimalType.Herbivore, 6000));
    }

    [Test]
    public async Task DeleteAnimalAsync_ValidId_ShouldReturnTrue()
    {
        // Arrange
        _animalRepositoryMock.Setup(repo => repo.DeleteAnimalAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _animalService.DeleteAnimalAsync(1);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task DeleteAnimalAsync_NonExistingId_ShouldReturnFalse()
    {
        // Arrange
        _animalRepositoryMock.Setup(repo => repo.DeleteAnimalAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _animalService.DeleteAnimalAsync(99);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task AddAnimalAsync_InvalidName_ShouldThrowArgumentException()
    {
        // Arrange
        var newAnimalDto = new AnimalDto { Name = string.Empty, Species = Consts.GiraffaCamelopardalis, Type = AnimalType.Herbivore, Class = AnimalClass.Mammal, Weight = 1200 };

        // Act and Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _animalService.AddAnimalAsync(newAnimalDto));
    }

    [Test]
    public async Task UpdateAnimalAsync_InvalidWeight_ShouldThrowArgumentException()
    {
        // Arrange
        var existingAnimal = new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 };
        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(new List<Animal> { existingAnimal });

        // Act and Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _animalService.UpdateAnimalAsync(1, Consts.Lioness, Consts.PantheraLeo, AnimalClass.Mammal, AnimalType.Carnivore, -50));
    }

    [Test]
    public async Task DeleteAnimalAsync_ExistingId_ShouldReturnTrue()
    {
        // Arrange
        _animalRepositoryMock.Setup(repo => repo.DeleteAnimalAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _animalService.DeleteAnimalAsync(1);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task GetSortedAnimalsAsync_EmptyAnimalList_ShouldReturnEmptyList()
    {
        // Arrange
        var animals = new List<Animal>(); // Empty list
        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(animals);

        // Act
        var result = await _animalService.GetSortedAnimalsAsync();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetSortedAnimalsAsync_OneAnimal_ShouldReturnSameAnimal()
    {
        // Arrange
        var animals = new List<Animal>
    {
        new Animal { Id = 1, Name = Consts.Lion, Type = AnimalType.Carnivore, Class = AnimalClass.Mammal, Weight = 200 }
    };
        _animalRepositoryMock.Setup(repo => repo.GetAnimalsAsync()).ReturnsAsync(animals);

        // Act
        var result = await _animalService.GetSortedAnimalsAsync();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(Consts.Lion, result[0].Name);
    }
}