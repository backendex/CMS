using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCMS.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // La llave secreta debe estar en tu appsettings.json
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
        }

        public async Task<string> CreateToken(User user, IList<string> roles)
        {
            // 1. Definimos los Claims básicos
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            // 2. Agregamos los Roles como Claims (React usará esto para permisos)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // OPCIONAL: Aquí podrías agregar permisos específicos según el rol
                // if(role == "Admin") claims.Add(new Claim("permission", "data.capture"));
            }

            // 3. Credenciales de firma
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Configuración del Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Issuer"],
                Audience = _config["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}