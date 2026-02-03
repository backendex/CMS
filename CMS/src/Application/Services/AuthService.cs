using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.DTOs.Post;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CMS.src.Application.DTOs.Auth.UserMethods;

namespace CMS.src.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RegisterByAdminAsync(RegisterDto registerDto)
        {
            var customRole = await _context.AccessRoles
                .FirstOrDefaultAsync(r => r.Id == registerDto.RolId);

            if (customRole == null)
                throw new Exception("El rol especificado no existe.");

            var tempPassword = GenerateRandomPassword();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            var rawToken = Guid.NewGuid().ToString();
            

            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Name = registerDto.Name,
                LastName = registerDto.LastName,
                RolId = customRole.Id,
                PasswordHash = hashedPassword,
                FullName = $"{registerDto.Name} {registerDto.LastName}",
                IsTemporaryPassword = true,
                IsActive = true,
                MustChangePassword = true,
                ValidationToken = rawToken
            };

            var full_name = user.Name + " " + user.LastName;

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al guardar en BD: {mensajeReal}");
            }

            try
            {
                var confirmationLink =
                    $"https://localhost:44351/api/auth/confirm-account?token={rawToken}";

                await _emailService.SendWelcomeEmail(
                    user.Email,
                    fullName : full_name,
                    tempPassword,
                    confirmationLink
                );
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
            }

            return true;
        }

        // UPDATE
        public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            user.FullName = dto.FullName;
            user.RolId = dto.RoleId;

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        // GET BY ID
        public async Task<UserResponseDto> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            return MapToDto(user);
        }

        //DELETE
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
                throw new Exception("Usuario no encontrado");

            user.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        //Mapear la info
        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = user.RolId
            };
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10) + "A1!";
        }                  
        public async Task<LoginResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.AccessRole)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null) 
                return new LoginResult
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return new LoginResult
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                };

            if (!user.IsActive)
                return new LoginResult
                {
                    Success = false,
                    Message = "Cuenta deshabilitada"
                };

            var token = GenerateToken(user);

            return new LoginResult
            {
                Success = true,
                Token = token,
                MustChangePassword = user.MustChangePassword,
                FullName = $"{user.Name} {user.LastName}",
                Role = user.AccessRole?.NameRol ?? "Sin Rol",
                Message = "Login OK"
            };
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.AccessRole)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (string.IsNullOrEmpty(user.PasswordHash)) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        private string GenerateToken(User user)
        {
            var secret = _configuration["JwtSettings:Key"];

            if (string.IsNullOrEmpty(secret))
                throw new Exception("La clave JWT no se encontró en la configuración.");

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ← clave
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("IsTemporary", user.IsTemporaryPassword.ToString())
            };


            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), 
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResponse> ActivateAccountAsync(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ValidationToken == token);

            if (user == null)
            {
                return new AuthResponse(false, "El enlace de activación es inválido o ya fue utilizado.", null);
            }

            user.IsActive = true;
            user.ValidationToken = null;

            await _context.SaveChangesAsync();

            return new AuthResponse(true, "¡Cuenta activada con éxito! Ya puedes iniciar sesión.", null);
        }

        public async Task<bool> ConfirmAccountAsync(string token)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ValidationToken == token);

            if (user == null)
                return false;

            user.IsActive = true;
            user.ValidationToken = null;
            user.MustChangePassword = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.Id)
                .ToListAsync();
        }        
        public async Task<AuthResult> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return new AuthResult { Success = false, Message = "Usuario no encontrado" };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.IsTemporaryPassword = false;
            user.MustChangePassword = false;
            user.IsActive = true;

            Console.WriteLine("ENTRÓ AL ENDPOINT CHANGE PASSWORD");

            try
            {
                await _context.SaveChangesAsync();
                return new AuthResult
                {
                    Success = true,
                    Message = "Contraseña actualizada correctamente"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Error en base de datos: " + ex.Message
                };
            }

            
        }

    }
}
