
namespace ZooAPI.Domain.Models.DTOs
{
    public class AnimalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public AnimalType Type { get; set; }
        public AnimalClass Class { get; set; }
        public decimal Weight { get; set; }
    }
}
