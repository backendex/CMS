using CMS.src.Application.DTOs.Auth;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using static CMS.src.Application.DTOs.Auth.UserMethods;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginDto dto);
        Task<bool> RegisterByAdminAsync(RegisterDto registerDto);
        Task<AuthResponse> ActivateAccountAsync(string token);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> ConfirmAccountAsync(string token);
        Task<IEnumerable<User>> GetAllUsersAsync(); 
        Task<AuthResult> ChangePasswordAsync(int v, string newPassword);
        Task UpdateUserAsync(User user);
        Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<UserResponseDto> GetByIdAsync(int id);
        Task DeleteAsync(int id);

    }
}
