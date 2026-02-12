using CMS.Infrastructure.Persistence;
using CMS.src.Application.Interfaces;
using CMS.src.Application.Services;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

#region DATABASE

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

// Inyección de tus servicios personalizados
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IEmailService, ResendEmailService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<ITourService, TourService>();
#endregion

#region JWT AUTHENTICATION

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSection["Key"]; 
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");
var secret = builder.Configuration["JwtSettings:Key"]; 
var keyBytes = Encoding.UTF8.GetBytes(secret);
var key = new SymmetricSecurityKey(keyBytes);

Console.WriteLine($"DEBUG: La clave leída es: {jwtKey}");

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("JwtSettings:Key no configurado en appsettings.json");
}
#endregion

#region AUTHORIZATION / POLICIES
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
            ),
            ClockSkew = TimeSpan.Zero
        };
    });
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:4321/")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region CONTROLLERS & SWAGGER

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce: Bearer {tu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

var app = builder.Build();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
#region MIDDLEWARE PIPELINE

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
