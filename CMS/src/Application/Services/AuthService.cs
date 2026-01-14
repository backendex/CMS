using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        //Registro de usuarios admin
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
                ValidationToken = rawToken
            };

            _context.Users.Add(user);

            try
            {
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var confirmationLink = $"https://localhost:44351/api/auth/confirm-account?email={user.Email}&token={rawToken}";
                    await _emailService.SendWelcomeEmail(user.Email, tempPassword, confirmationLink);

                    return true;
                }
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException?.Message;
                throw;
            }

            return false;
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

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                string myJwt = GenerateToken(user);

                var full_name = user.Name + " " + user.LastName;
                var name_rol = user.AccessRole?.NameRol ?? "Sin Rol";

                return new LoginResult
                {
                    Success = true,
                    Token = myJwt,
                    MustChangePassword = user.IsTemporaryPassword,
                    FullName = full_name, 
                    Role = name_rol,
                    Message = "Login OK"
                };
            }

            return new LoginResult { Success = false, Message = "Credenciales inválidas" };
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
            // 1. Extraer la clave exactamente igual que en Program.cs
            var secret = _configuration["JwtSettings:Key"];

            // Validación de seguridad por si el archivo no se lee
            if (string.IsNullOrEmpty(secret))
                throw new Exception("La clave JWT no se encontró en la configuración.");

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Definir los Claims (Asegúrate de importar System.IdentityModel.Tokens.Jwt)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único del token
                new Claim("role", "Admin"), // Coincide con RoleClaimType = "role"
                new Claim("IsTemporary", user.IsTemporaryPassword.ToString())
            };

            // 3. Crear el token usando JwtSettings
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Importante usar UtcNow
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

        public async Task<bool> ConfirmAccountAsync(string email, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.ValidationToken == token);

            if (user == null) return false;

            user.IsActive = true;         
            user.ValidationToken = null;   

            await _context.SaveChangesAsync();

            //console log para depurar y visualizar que esta devolviendo
            Console.WriteLine($"Usuario {user.Email} activado correctamente. IsActive en memoria: {user.IsActive}");
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
