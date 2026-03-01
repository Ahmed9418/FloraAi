using FloraAI.Data;
using FloraAI.DTOs;
using FloraAI.Interfaces;
using FloraAI.Models;
using FloraAI.Patterns;
using Microsoft.EntityFrameworkCore;

namespace FloraAI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext         _db;
    private readonly JwtTokenProvider     _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, JwtTokenProvider jwt, ILogger<AuthService> logger)
    { _db = db; _jwt = jwt; _logger = logger; }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
            throw new InvalidOperationException($"البريد الإلكتروني '{request.Email}' مسجّل مسبقاً.");

        var user = new User
        {
            FirstName    = request.FirstName.Trim(),
            LastName     = request.LastName.Trim(),
            Email        = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");

        var (token, expiresAt) = _jwt.GenerateToken(user);
        return new AuthResponse(user.Id, $"{user.FirstName} {user.LastName}", user.Email, token, expiresAt);
    }
}