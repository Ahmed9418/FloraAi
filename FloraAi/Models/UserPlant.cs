namespace FloraAI.Models;

public class UserPlant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PlantLibraryId { get; set; }
    public string? NickName { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime NextWateringDate { get; set; }

    public User? User { get; set; }
    public PlantLibrary? PlantLibrary { get; set; }
    public ICollection<DiagnosisRecord> DiagnosisRecords { get; set; } = new List<DiagnosisRecord>();
}