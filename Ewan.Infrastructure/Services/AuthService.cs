using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ewan.Application.DTOs.Auth;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ewan.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly EwanDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<AdminUser> _passwordHasher = new();

    public AuthService(EwanDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.AdminUsers.FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower() && u.IsActive);
        if (user is null) return null;

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password.Trim());
        if (verifyResult == PasswordVerificationResult.Failed) return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return GenerateToken(user);
    }

    private LoginResponse GenerateToken(AdminUser user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // "uid" هو نفس الـ Claim اللي كل الـ Controllers بتقرأ منه GetCurrentUserId()
        // والـ Role claim العادي (ClaimTypes.Role) هو اللي [Authorize(Roles = "...")] بيعتمد عليه
        var claims = new List<Claim>
        {
            new("uid", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var expiresAt = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }
}