using CMS.Application.Services;
using CMS.Infrastructure.Services;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CMS.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. BASE DE DATOS (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ---------------------------------------------------------
// 2. ELIMINADO: builder.Services.AddIdentityCore...
// No lo usamos porque tu clase 'Users' no hereda de IdentityUser.
// ---------------------------------------------------------

// 3. INYECCIÓN DE DEPENDENCIAS
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. CONFIGURACIÓN DE AUTENTICACIÓN JWT
// Validamos que el TokenKey exista para evitar errores de referencia nula
var tokenKey = builder.Configuration["TokenKey"]
    ?? throw new Exception("TokenKey no encontrado en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 5. CONTROLADORES Y SWAGGER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración de Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// El orden es vital: Authentication antes que Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();