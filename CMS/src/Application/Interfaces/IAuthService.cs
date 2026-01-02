using CMS.src.Application.DTOs;

namespace CMS.src.Application.Interfaces
{
    public interface IAuthService
    {
        // Método para registrar nuevos usuarios con un rol específico
        Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);

        // Método para validar credenciales y devolver el token con permisos
        Task<AuthResponseDto> LoginAsync(LoginRequest loginDto);
    }
}
