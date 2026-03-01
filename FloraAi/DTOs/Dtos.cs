using System.ComponentModel.DataAnnotations;

namespace FloraAI.DTOs;

// ── AUTH ──────────────────────────────────────────────────────────────────────
public record RegisterRequest(
    [Required, MaxLength(100)] string FirstName,
    [Required, MaxLength(100)] string LastName,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8)] string Password
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(int UserId, string FullName, string Email, string Token, DateTime ExpiresAt);

// ── WORKFLOW 1: تحليل النبتة ──────────────────────────────────────────────────
public record DiagnosePlantRequest(
    [Required] int UserId,
    [Required, MaxLength(150)] string PlantName,
    [Required] string DiseaseLabel,
    int? UserPlantId = null
);

public record DiagnosePlantResponse(
    int DiagnosisId,
    string PlantName,
    string ArabicDiseaseLabel,
    string TreatmentText,
    DateTime DiagnosedAt,
    bool SavedToGarden
);

// ── WORKFLOW 2: حديقتي ────────────────────────────────────────────────────────
public record AddPlantToGardenRequest(
    [Required] int UserId,
    [Required, MaxLength(150)] string PlantName,
    [MaxLength(100)] string? NickName = null
);

public record CareScheduleDto(
    string PlantName,
    string ArabicName,
    string WateringInstructions,
    string SunlightRequirement,
    string FertilizingInstructions,
    string CareTips,
    DateTime NextWateringDate
);

public record DiagnosisHistoryItemDto(
    int DiagnosisId,
    string ArabicDiseaseLabel,
    string TreatmentText,
    DateTime DiagnosedAt
);

public record PlantProfileResponse(
    int UserPlantId,
    string NickName,
    CareScheduleDto CareSchedule,
    List<DiagnosisHistoryItemDto> HealthRecord
);

public record MyGardenCardDto(
    int UserPlantId,
    string NickName,
    string ArabicName,
    DateTime NextWateringDate,
    string? LastDiseaseLabel
);

// ── GEMINI INTERNAL ───────────────────────────────────────────────────────────
public record GeminiNewPlantResponse(
    int BaseWateringDays,
    string WateringInstructions,
    string SunlightRequirement,
    string FertilizingInstructions,
    string CareTips,
    string ArabicName
);