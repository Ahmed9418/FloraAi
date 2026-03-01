using FloraAI.Data;
using FloraAI.DTOs;
using FloraAI.Interfaces;
using FloraAI.Models;
using Microsoft.EntityFrameworkCore;

namespace FloraAI.Services;

public class GardenService : IGardenService
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;
    private readonly ILogger<GardenService> _logger;

    public GardenService(AppDbContext db, IGeminiService gemini, ILogger<GardenService> logger)
    { _db = db; _gemini = gemini; _logger = logger; }

    public async Task<PlantProfileResponse> AddPlantAsync(AddPlantToGardenRequest request)
    {
        var normalized = request.PlantName.ToLower().Trim();
        var plantEntry = await _db.PlantLibrary.FirstOrDefaultAsync(p => p.Name == normalized);

        if (plantEntry is null)
        {
            var aiData = await _gemini.GenerateNewPlantDataAsync(request.PlantName);
            plantEntry = new PlantLibrary
            {
                Name = normalized,
                ArabicName = aiData?.ArabicName ?? request.PlantName,
                BaseWateringDays = aiData?.BaseWateringDays ?? 7,
                WateringInstructions = aiData?.WateringInstructions ?? "اسقِ النبات كل 7 أيام.",
                SunlightRequirement = aiData?.SunlightRequirement ?? "ضوء غير مباشر متوسط.",
                FertilizingInstructions = aiData?.FertilizingInstructions ?? "سماد سائل كل شهر في موسم النمو.",
                CareTips = aiData?.CareTips ?? "راقب النبات دورياً.",
                IsAiGenerated = true
            };
            _db.PlantLibrary.Add(plantEntry);
            await _db.SaveChangesAsync();
        }

        var userPlant = new UserPlant
        {
            UserId = request.UserId,
            PlantLibraryId = plantEntry.Id,
            NickName = string.IsNullOrWhiteSpace(request.NickName) ? plantEntry.ArabicName : request.NickName,
            NextWateringDate = DateTime.UtcNow.AddDays(plantEntry.BaseWateringDays)
        };

        _db.UserPlants.Add(userPlant);
        await _db.SaveChangesAsync();
        return BuildProfile(userPlant, plantEntry, new List<DiagnosisRecord>());
    }

    public async Task<List<MyGardenCardDto>> GetMyGardenAsync(int userId)
    {
        var plants = await _db.UserPlants
            .Include(up => up.PlantLibrary)
            .Include(up => up.DiagnosisRecords.OrderByDescending(d => d.DiagnosedAt).Take(1))
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.AddedAt)
            .ToListAsync();

        return plants.Select(up => new MyGardenCardDto(
            up.Id,
            up.NickName ?? up.PlantLibrary!.ArabicName,
            up.PlantLibrary!.ArabicName,
            up.NextWateringDate,
            up.DiagnosisRecords.FirstOrDefault()?.ArabicDiseaseLabel
        )).ToList();
    }

    public async Task<PlantProfileResponse> GetPlantProfileAsync(int userPlantId)
    {
        var up = await _db.UserPlants
            .Include(u => u.PlantLibrary)
            .Include(u => u.DiagnosisRecords.OrderByDescending(d => d.DiagnosedAt))
            .FirstOrDefaultAsync(u => u.Id == userPlantId)
            ?? throw new KeyNotFoundException($"النبات رقم {userPlantId} غير موجود.");

        return BuildProfile(up, up.PlantLibrary!, up.DiagnosisRecords.ToList());
    }

    private static PlantProfileResponse BuildProfile(UserPlant up, PlantLibrary p, List<DiagnosisRecord> records) =>
        new(
            up.Id,
            up.NickName ?? p.ArabicName,
            new CareScheduleDto(p.Name, p.ArabicName, p.WateringInstructions,
                p.SunlightRequirement, p.FertilizingInstructions, p.CareTips, up.NextWateringDate),
            records.Select(d => new DiagnosisHistoryItemDto(d.Id, d.ArabicDiseaseLabel, d.TreatmentText, d.DiagnosedAt)).ToList()
        );
}