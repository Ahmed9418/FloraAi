using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FloraAI.Models;
using Microsoft.IdentityModel.Tokens;

namespace FloraAI.Patterns;

public sealed class JwtTokenProvider
{
    private static JwtTokenProvider? _instance;
    private static readonly object _lock = new();

    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryHours;

    private JwtTokenProvider(IConfiguration config)
    {
        _secretKey = config["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey غير مضبوط.");
        _issuer = config["Jwt:Issuer"] ?? "FloraAI";
        _audience = config["Jwt:Audience"] ?? "FloraAIApp";
        _expiryHours = int.TryParse(config["Jwt:ExpiryHours"], out var h) ? h : 24;
    }

    public static JwtTokenProvider GetInstance(IConfiguration config)
    {
        if (_instance is null)
            lock (_lock)
                _instance ??= new JwtTokenProvider(config);
        return _instance;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_expiryHours);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email,      user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName,  user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_issuer, _audience, claims, expires: expiresAt, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}