using System.Text;
using System.Text.Json;
using FloraAI.DTOs;
using FloraAI.Interfaces;

namespace FloraAI.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<GeminiService> _logger;

    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    public GeminiService(IHttpClientFactory factory, IConfiguration config, ILogger<GeminiService> logger)
    {
        _http = factory.CreateClient("GeminiClient");
        _config = config;
        _logger = logger;
    }

    public async Task<string> GenerateArabicTreatmentTextAsync(string plantName, string diseaseLabel)
    {
        var result = await CallGeminiAsync(BuildTreatmentPrompt(plantName, diseaseLabel), jsonMode: false);
        return result ?? BuildFallbackText(plantName, diseaseLabel);
    }

    public async Task<GeminiNewPlantResponse?> GenerateNewPlantDataAsync(string plantName)
    {
        var raw = await CallGeminiAsync(BuildNewPlantPrompt(plantName), jsonMode: true);
        if (raw is null) return null;
        try
        {
            return JsonSerializer.Deserialize<GeminiNewPlantResponse>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل تحليل JSON لبيانات النبات '{Plant}'.", plantName);
            return null;
        }
    }

    private async Task<string?> CallGeminiAsync(string prompt, bool jsonMode)
    {
        var apiKey = _config["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) { _logger.LogError("Gemini:ApiKey غير مضبوط."); return null; }

        var body = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = jsonMode
                ? new { temperature = 0.1, maxOutputTokens = 1024, responseMimeType = "application/json" }
                : new { temperature = 0.3, maxOutputTokens = 1024, responseMimeType = "text/plain" }
        };

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"{BaseUrl}?key={apiKey}", content);
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var text = doc.RootElement
                .GetProperty("candidates")[0].GetProperty("content")
                .GetProperty("parts")[0].GetProperty("text").GetString();

            if (jsonMode && text is not null)
            {
                text = text.Trim().TrimStart('`').TrimEnd('`');
                if (text.StartsWith("json", StringComparison.OrdinalIgnoreCase)) text = text[4..].Trim();
            }
            return text;
        }
        catch (Exception ex) { _logger.LogError(ex, "فشل استدعاء Gemini API."); return null; }
    }

    private static string BuildTreatmentPrompt(string plantName, string diseaseLabel)
    {
        var arabicDisease = diseaseLabel.ToLower() switch
        {
            "fungi" => "فطريات",
            "bacteria" => "بكتيريا",
            "virus" => "فيروس",
            "pests" => "آفات حشرية",
            _=> "سليمة"
        };

        return $$"""
        You are a professional indoor plant expert. Your task is to write a diagnostic report in a warm, engaging, and narrative Arabic style.

        DATA:
        - Plant Name: {{plantName}}
        - Diagnosis: {{arabicDisease}}

        INSTRUCTIONS:
        1. Write the response as continuous paragraphs in a narrative or storytelling tone.
        2. DO NOT use subheadings, bullet points, or Markdown formatting (like stars **).
        3. Start with a warm greeting, explain the plant's condition, and integrate treatment steps, care advice, and recovery time naturally within the narrative flow.
        4. The tone should feel like a personal message from an expert to a hobbyist friend.
        5. If the plant is "Healthy" (سليمة), write a congratulatory message full of positive energy, including general preventive care tips woven into the text.

        OUTPUT REQUIREMENT: Return ONLY the narrative Arabic text.
        """;
    }
    private static string BuildNewPlantPrompt(string plantName) => $$"""
    You are a strict botanical expert API. Your ONLY output must be a single valid JSON object with NO markdown, NO backticks, NO explanation.

    Plant Name: {{plantName}}

    IMPORTANT: All string values MUST be written in the Arabic language (العربية). The JSON keys MUST remain in English.

    Return this exact JSON structure:
    {
      "BaseWateringDays": <integer>,
      "WateringInstructions": "<One sentence in Arabic explaining watering>",
      "SunlightRequirement": "<One sentence in Arabic explaining sunlight>",
      "FertilizingInstructions": "<One sentence in Arabic explaining fertilizer>",
      "CareTips": "<One sentence in Arabic with a care tip>",
      "ArabicName": "<The common Arabic name for the plant>"
    }
    """;

    private static string BuildFallbackText(string plantName, string diseaseLabel)
    {
        var arabicDisease = diseaseLabel.ToLower() switch
        {
            "fungi" => "فطريات",
            "bacteria" => "بكتيريا",
            "virus" => "فيروس",
            "pests" => "آفات حشرية",
            _=> "صحي"
        };
        if (diseaseLabel.ToLower() == "healthy")
            return $"نبات {plantName} يبدو بصحة جيدة. استمر في روتين الرعاية المنتظم وراقب النبات دورياً.";

        return $"""
            التشخيص:
            تم اكتشاف إصابة بـ{arabicDisease} في نبات {plantName}.
            خطوات العلاج الفوري:
            1. عزل النبات فوراً عن باقي النباتات.
            2. إزالة الأوراق المصابة بمقص معقّم.
            3. تطبيق العلاج المناسب لـ{arabicDisease}.
            4. المراقبة اليومية لمدة أسبوعين.
            تعديل الرعاية خلال التعافي:
            قلّل الري وتجنّب التسميد خلال فترة العلاج. تأكد من وجود تهوية جيدة.
            تحذيرات السلامة:
            ارتدِ قفازات عند التعامل مع المبيدات. أبعد النبات عن الأطفال والحيوانات.
            مدة التعافي المتوقعة:
            من أسبوعين إلى ثلاثة أسابيع مع الالتزام بالعلاج.
            """;
    }
}