namespace FloraAI.Models;

public class DiagnosisRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? UserPlantId { get; set; }
    public required string PlantNameInput { get; set; }
    public required string DiseaseLabel { get; set; }
    public required string ArabicDiseaseLabel { get; set; }
    public required string TreatmentText { get; set; }
    public DateTime DiagnosedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public UserPlant? UserPlant { get; set; }
}