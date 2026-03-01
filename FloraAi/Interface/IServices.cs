using FloraAI.DTOs;
using FloraAI.Models;

namespace FloraAI.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public interface IGeminiService
{
    Task<string> GenerateArabicTreatmentTextAsync(string plantName, string diseaseLabel);
    Task<GeminiNewPlantResponse?> GenerateNewPlantDataAsync(string plantName);
}

public interface IDiagnosisService
{
    Task<DiagnosePlantResponse> DiagnoseAsync(DiagnosePlantRequest request);
    Task<List<DiagnosisHistoryItemDto>> GetHealthRecordAsync(int userPlantId);
}

public interface IGardenService
{
    Task<PlantProfileResponse> AddPlantAsync(AddPlantToGardenRequest request);
    Task<List<MyGardenCardDto>> GetMyGardenAsync(int userId);
    Task<PlantProfileResponse> GetPlantProfileAsync(int userPlantId);
}