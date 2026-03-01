using FloraAI.Data;
using FloraAI.DTOs;
using FloraAI.Interfaces;
using FloraAI.Models;
using FloraAI.Patterns;
using Microsoft.EntityFrameworkCore;

namespace FloraAI.Services;

public class DiagnosisService : IDiagnosisService
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;
    private readonly ILogger<DiagnosisService> _logger;

    public DiagnosisService(AppDbContext db, IGeminiService gemini, ILogger<DiagnosisService> logger)
    { _db = db; _gemini = gemini; _logger = logger; }

    public async Task<DiagnosePlantResponse> DiagnoseAsync(DiagnosePlantRequest request)
    {
        var strategy = DiseaseStrategyFactory.Create(request.DiseaseLabel);
        var treatmentText = await _gemini.GenerateArabicTreatmentTextAsync(request.PlantName, request.DiseaseLabel);

        var record = new DiagnosisRecord
        {
            UserId = request.UserId,
            UserPlantId = request.UserPlantId,
            PlantNameInput = request.PlantName.Trim(),
            DiseaseLabel = request.DiseaseLabel.ToLower(),
            ArabicDiseaseLabel = strategy.ArabicLabel,
            TreatmentText = treatmentText,
            DiagnosedAt = DateTime.UtcNow
        };

        _db.DiagnosisRecords.Add(record);

        if (request.UserPlantId.HasValue)
        {
            var userPlant = await _db.UserPlants.FindAsync(request.UserPlantId.Value);
            if (userPlant is not null)
            {
                var baseDays = (await _db.PlantLibrary.FindAsync(userPlant.PlantLibraryId))?.BaseWateringDays ?? 7;
                userPlant.NextWateringDate = DateTime.UtcNow.AddDays(strategy.GetRecoveryWateringDays(baseDays));
            }
        }

        await _db.SaveChangesAsync();
        return new DiagnosePlantResponse(record.Id, record.PlantNameInput, record.ArabicDiseaseLabel,
            treatmentText, record.DiagnosedAt, request.UserPlantId.HasValue);
    }

    public async Task<List<DiagnosisHistoryItemDto>> GetHealthRecordAsync(int userPlantId)
    {
        var records = await _db.DiagnosisRecords
            .Where(d => d.UserPlantId == userPlantId)
            .OrderByDescending(d => d.DiagnosedAt)
            .ToListAsync();

        return records.Select(d => new DiagnosisHistoryItemDto(
            d.Id, d.ArabicDiseaseLabel, d.TreatmentText, d.DiagnosedAt)).ToList();
    }
}