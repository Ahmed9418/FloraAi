namespace FloraAI.Models;

public class PlantLibrary
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ArabicName { get; set; }
    public int BaseWateringDays { get; set; }
    public required string WateringInstructions { get; set; }
    public required string SunlightRequirement { get; set; }
    public required string FertilizingInstructions { get; set; }
    public required string CareTips { get; set; }
    public bool IsAiGenerated { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
}