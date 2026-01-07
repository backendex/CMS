using CMS.src.Application.DTOs.Auth;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
        Task<AuthResponse> ActivateAccountAsync(string token);
        Task<IdentityResult> RegisterByAdminAsync(RegisterDto registerDto);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<string> GenerateJwtTokenAsync(User user);

    }
}
