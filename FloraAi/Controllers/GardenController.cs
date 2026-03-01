using FloraAI.DTOs;
using FloraAI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloraAI.Controllers;

[ApiController, Route("api/garden"), Authorize, Produces("application/json")]
public class GardenController : ControllerBase
{
    private readonly IGardenService _garden;
    public GardenController(IGardenService garden) => _garden = garden;

    /// <summary>إضافة نبات لحديقة المستخدم.</summary>
    [HttpPost("add")]
    public async Task<IActionResult> AddPlant([FromBody] AddPlantToGardenRequest request) =>
        Ok(await _garden.AddPlantAsync(request));

    /// <summary>قائمة نباتات الحديقة — بطاقات مختصرة.</summary>
    [HttpGet("my-garden/{userId:int}")]
    public async Task<IActionResult> GetMyGarden([FromRoute] int userId) =>
        Ok(await _garden.GetMyGardenAsync(userId));

    /// <summary>ملف النبات الكامل: جدول الرعاية + السجل الصحي.</summary>
    [HttpGet("plant-profile/{userPlantId:int}")]
    public async Task<IActionResult> GetPlantProfile([FromRoute] int userPlantId)
    {
        try { return Ok(await _garden.GetPlantProfileAsync(userPlantId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { خطأ = ex.Message }); }
    }
}