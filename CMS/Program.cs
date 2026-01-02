using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyCMS.Application.Services;
using MyCMS.Infrastructure.Persistence;
using MyCMS.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. SERVICIOS DE INFRAESTRUCTURA (Base de Datos)
// ---------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Aquí también configurarías la autenticación usando builder.Configuration["TokenKey"]

// ---------------------------------------------------------
// 2. CONFIGURACIÓN DE IDENTITY (Seguridad de Usuarios/Roles)
// ---------------------------------------------------------
builder.Services.AddIdentityCore<User>(opt => {
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>() // IMPORTANTE: Para manejar roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// ---------------------------------------------------------
// 3. INYECCIÓN DE DEPENDENCIAS (Conectando Capas)
// ---------------------------------------------------------
// Unimos las Interfaces de 'Application' con las clases de 'Infrastructure'
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ---------------------------------------------------------
// 4. CONFIGURACIÓN DE AUTENTICACIÓN JWT
// ---------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 5. CONTROLADORES Y SWAGGER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Agrega esto para que Swagger funcione en el navegador
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API V1");
        c.RoutePrefix = "swagger"; // Esto hace que entres por localhost:XXXX/swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // ¡Vital para que no dé 404!

app.Run();
