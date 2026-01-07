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
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager,IApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (userExists) return new AuthResponse(false, "El correo ya existe.", null);

            var newUser = new User
            {
                Email = registerDto.Email,
                RolId = registerDto.RolId,

                // Configuración inicial
                IsActive = false,
                ValidationToken = Guid.NewGuid().ToString() 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();          

            string activationLink = $"https://localhost:44351/api/Auth/activate?token={newUser.ValidationToken}";

            string htmlBody = $@"
        <div style='font-family: sans-serif; padding: 20px; border: 1px solid #ddd;'>
            <h2>Confirma tu cuenta</h2>
            <p>Gracias por unirte al CMS. Haz clic en el botón para activar tu acceso:</p>
            <a href='{activationLink}' style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Activar Cuenta
            </a>
        </div>";

            await _emailService.SendEmailAsync(newUser.Email, "Activa tu cuenta de CMS", htmlBody);

            return new AuthResponse(true, "Usuario registrado. Revise su correo para activar su cuenta.", null);
        }
        public async Task<IdentityResult> RegisterByAdminAsync(RegisterDto model)
        {
            var customRole = await _context.AccessRoles
                .FirstOrDefaultAsync(r => r.Id == model.RolId);

            if (customRole == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "El rol especificado no existe." });
            }

            var tempPassword = GenerateRandomPassword();

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                RolId = customRole.Id,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, tempPassword);

            if (result.Succeeded)
            {

                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, customRole.NameRol));

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = $"https://tu-frontend.com/confirmar?userId={user.Id}&token={encodedToken}";

                await _emailService.SendWelcomeEmail(user.Email, tempPassword, confirmationLink);

                return result;
            }
            return result;
        }
        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10) + "A1!";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Credenciales inválidas");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Cuenta no confirmada");

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            return await GenerateJwtTokenAsync(user);
        }

        public async Task<User?> FindByEmailAsync(string email)
         => await _userManager.FindByEmailAsync(email);

        public async Task<bool> CheckPasswordAsync(User user, string password)
            => await _userManager.CheckPasswordAsync(user, password);

  
        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
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

        Task<AuthResponse> IAuthService.LoginAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }
    }
}
