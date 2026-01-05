using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
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

        //Aqui se define la funcion para el registro
        public async Task<AuthResponse> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Usamos tu entidad 'User' del Domain, no ApplicationUser
            var userExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (userExists) return new AuthResponse(false, "El correo ya existe.", null);

            var newUser = new User
            {
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                RolId = registerDto.RolId,

                // Configuración inicial
                IsActive = false,
                ValidationToken = Guid.NewGuid().ToString() // Genera un código único
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // 4. Preparar y enviar el correo (Usando el IEmailService inyectado)
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

            
        
        //Aqui se define la funcion para el login
        public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
        {
            // Buscamos al usuario incluyendo su Rol para los permisos del Token
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // 1. Validar credenciales
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return new AuthResponse(false, "Correo o contraseña incorrectos.", null);
            }

            // 2. Validar si la cuenta está activa (EL CAMPO QUE AGREGAMOS)
            if (!user.IsActive)
            {
                return new AuthResponse(false, "Tu cuenta aún no ha sido activada. Revisa tu correo.", null);
            }

            // 3. Generar el Token si todo está bien
            var token = GenerateJwtToken(user);

            return new AuthResponse(true, "Sesión iniciada correctamente", token);
        }
        //Función para generar el token JWT
        private string GenerateJwtToken(User user)
        {
            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<AuthResponse> ActivateAccountAsync(string token)
        {
            // Buscamos al usuario que tenga ese token de validación
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ValidationToken == token);

            if (user == null)
            {
                return new AuthResponse(false, "El enlace de activación es inválido o ya fue utilizado.", null);
            }

            // Activamos la cuenta y limpiamos el token
            user.IsActive = true;
            user.ValidationToken = null;

            await _context.SaveChangesAsync();

            return new AuthResponse(true, "¡Cuenta activada con éxito! Ya puedes iniciar sesión.", null);
        }

    
    }
}
