using FloraAI.DTOs;
using FloraAI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloraAI.Controllers;

[ApiController, Route("api/diagnosis"), Authorize, Produces("application/json")]
public class DiagnosisController : ControllerBase
{
    private readonly IDiagnosisService _diagnosis;
    public DiagnosisController(IDiagnosisService diagnosis) => _diagnosis = diagnosis;

    /// صفحة تحليل النبتة — يُعيد خطة العلاج بالعربية كنص جاهز للعرض.
    /// أرسل UserPlantId لحفظ التشخيص في السجل الصحي لنبات في الحديقة.
    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] DiagnosePlantRequest request)
    {
        try { return Ok(await _diagnosis.DiagnoseAsync(request)); }
        catch (ArgumentException ex) { return BadRequest(new { خطأ = ex.Message }); }
    }

    /// <summary>تبويب السجل الصحي لنبات في الحديقة.</summary>
    [HttpGet("health-record/{userPlantId:int}")]
    public async Task<IActionResult> GetHealthRecord([FromRoute] int userPlantId) =>
        Ok(await _diagnosis.GetHealthRecordAsync(userPlantId));
}