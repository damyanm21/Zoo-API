using ZooAPI.Domain.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public AnimalType Type { get; set; }
    public AnimalClass Class { get; set; }
    public decimal Weight { get; set; }
    public decimal FoodNeeded { get; set; }
}
