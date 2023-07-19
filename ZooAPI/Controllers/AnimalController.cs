using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ZooAPI.BusinessLogic;
using ZooAPI.Domain.Models.DTOs;

namespace ZooAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly IMapper _mapper;

        public AnimalController(IAnimalService animalService, IMapper mapper)
        {
            _animalService = animalService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            var animals = await _animalService.GetSortedAnimalsAsync();
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimalById(int id)
        {
            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            return Ok(animal);
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddAnimal(AnimalDto animal)
        {
            try
            {
                var animalId = await _animalService.AddAnimalAsync(animal);
                return CreatedAtAction(nameof(GetAnimalById), new { id = animalId }, animalId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAnimal(int id, AnimalDto animal)
        {
            try
            {
                var success = await _animalService.UpdateAnimalAsync(id, animal.Name, animal.Species, animal.Class, animal.Type, animal.Weight);
                if (success)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnimal(int id)
        {
            var success = await _animalService.DeleteAnimalAsync(id);
            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
