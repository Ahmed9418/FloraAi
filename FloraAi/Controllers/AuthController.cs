using FloraAI.DTOs;
using FloraAI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FloraAI.Controllers;

[ApiController, Route("api/auth"), Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        try 
        { 
            var u = await _auth.RegisterAsync(req); 
            return StatusCode(201, new { رسالة = "تم إنشاء الحساب بنجاح.", معرّفالمستخدم = u.Id }); 
        }
        catch (InvalidOperationException ex) { return Conflict(new { خطأ = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try { return Ok(await _auth.LoginAsync(req)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { خطأ = ex.Message }); }
    }
}