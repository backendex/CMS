using CMS.src.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
        Task<AuthResponse> ActivateAccountAsync(string token);

    }
}
