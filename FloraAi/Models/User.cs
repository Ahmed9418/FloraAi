namespace FloraAI.Models;

public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    public ICollection<DiagnosisRecord> DiagnosisRecords { get; set; } = new List<DiagnosisRecord>();
}