using CMS.src.Application.DTOs.Auth;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginDto dto);
        Task<bool> RegisterByAdminAsync(RegisterDto registerDto);
        Task<AuthResponse> ActivateAccountAsync(string token);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> ConfirmAccountAsync(string email, string token);
        Task<IEnumerable<User>> GetAllUsersAsync(); // O usa un DTO si no quieres exponer toda la entidad
        Task<AuthResult> ChangePasswordAsync(string email, string currentPassword, string newPassword);
    }
}
