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
                Name = registerDto.Name,
                LastName = registerDto.LastName,
                RolId = customRole.Id,
                PasswordHash = hashedPassword,
                IsTemporaryPassword = true,
                IsActive = false,
                ValidationToken = rawToken

            };

            _context.Users.Add(user);
            //Aqui hay un bug
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var confirmationLink = $"https://localhost:44351/api/auth/confirm-account?email={user.Email}&token={rawToken}";
                await _emailService.SendWelcomeEmail(user.Email, tempPassword, confirmationLink);

                return true;
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
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                string myJwt = GenerateToken(user);

                return new LoginResult
                {
                    Success = true,
                    Token = myJwt,
                    MustChangePassword = user.IsTemporaryPassword
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
            var claims = new[]
    {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsTemporary", user.IsTemporaryPassword.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ValidationToken == token);

            if (user == null) return false;

            user.IsActive = true;         
            user.ValidationToken = null;   

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
