using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs.Auth;
using MyWebApi.Services;

namespace MyWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ============================
    // REGISTER
    // POST: /api/auth/register
    // ============================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == "Email already exists")
        {
            return BadRequest(new
            {
                message = result
            });
        }

        if (result == "Default User role not found")
        {
            return BadRequest(new
            {
                message = result
            });
        }

        return Ok(new
        {
            message = result
        });
    }

    // ============================
    // LOGIN
    // POST: /api/auth/login
    // ============================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password"
            });
        }

        return Ok(new
        {
            message = "Login successful",
            token = token
        });
    }
}