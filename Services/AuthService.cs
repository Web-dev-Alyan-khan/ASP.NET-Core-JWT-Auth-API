using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWebApi.Data;
using MyWebApi.DTOs.Auth;
using MyWebApi.Helpers;
using MyWebApi.Models;

namespace MyWebApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext context,
        PasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    // ============================
    // REGISTER
    // ============================

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            return "Email already exists";
        }

        // Find default User role
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "User");

        if (role == null)
        {
            return "Default User role not found";
        }

        // Create user
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            RoleId = role.RoleId
        };

        // Hash password
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password
        );

        // Save user
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return "User registered successfully";
    }

    // ============================
    // LOGIN
    // ============================

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        // Find user with role
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return null;
        }

        // Verify password
        var passwordValid = _passwordHasher.VerifyPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (!passwordValid)
        {
            return null;
        }

        // Generate JWT
        return GenerateToken(user);
    }

    // ============================
    // JWT TOKEN
    // ============================

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role?.RoleName ?? "User"
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(
                    _configuration["Jwt:ExpiryMinutes"]!
                )
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}