using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context; // Inyectamos el contexto directamente
        private readonly ITokenService _tokenService;

        // ELIMINA el UserManager de aquí
        public AuthService(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public Task<AuthResponseDto> LoginAsync(LoginRequest loginDto)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterAsync(UserRegisterDto registerDto)
        {
            // Ahora usas _context.Users para interactuar con la DB
            var user = new User
            {
                Email = registerDto.Email,
                Password = registerDto.Password, // Deberías usar un hash aquí
                RolId = 1 // Ajustado a tu columna rol_id de la imagen
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        Task<AuthResponseDto> IAuthService.RegisterAsync(UserRegisterDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}