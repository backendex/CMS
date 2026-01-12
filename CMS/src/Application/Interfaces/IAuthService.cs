using CMS.src.Application.DTOs.Auth;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginDto dto);
        Task<AuthResponse> RegisterAsync(RegisterDto dto);
        Task<IdentityResult> RegisterByAdminAsync(RegisterDto dto);
        Task<AuthResponse> ActivateAccountAsync(string token);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);

    }
}
