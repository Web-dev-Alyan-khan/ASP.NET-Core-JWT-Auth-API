using MyWebApi.DTOs.Auth;

namespace MyWebApi.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}