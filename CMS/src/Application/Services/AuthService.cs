using CMS.src.Application.DTOs;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MyCMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto)
        {

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            // 2. Guardar en la base de datos
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 3. Asignar el Rol (Admin, Editor, etc.)
            await _userManager.AddToRoleAsync(user, registerDto.Role);

            return new AuthResponseDto { IsSuccess = true, Message = "Usuario registrado exitosamente" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest loginDto)
        {
            // 1. Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return new AuthResponseDto { IsSuccess = false, Message = "Usuario no encontrado" };

            // 2. Validar contraseña
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result) return new AuthResponseDto { IsSuccess = false, Message = "Contraseña incorrecta" };

            // 3. Obtener los roles para incluirlos en los Claims del Token
            var roles = await _userManager.GetRolesAsync(user);

            // 4. Generar el Token JWT
            var token = await _tokenService.CreateToken(user, roles);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "No Role"
            };
        }
    }
}