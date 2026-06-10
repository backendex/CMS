using CMS.Infrastructure.Persistence;
using CMS.src.Application.Interfaces;
using CMS.src.Application.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CMS.src.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

public static class Program
{
    // Plan (pseudocódigo, detallado):
    // 1. Mover todo el código de nivel superior a un punto de entrada explícito `Main`.
    // 2. Mantener todas las declaraciones `using` y líneas existentes intactas.
    // 3. Dentro de `Main`:
    //    a. Limpiar el mapeo de claims por defecto.
    //    b. Construir el `WebApplicationBuilder`.
    //    c. Configurar la base de datos, servicios y fábrica de contexto.
    //    d. Configurar autenticación JWT y políticas.
    //    e. Configurar CORS, controllers y Swagger.
    //    f. Construir la aplicación, configurar middleware y ejecutar con `RunAsync`.
    // 4. Usar `async Task Main` y `await app.RunAsync()` para permitir correcta finalización asíncrona.
    // Resultado: evitar el diagnóstico ENC0118 moviendo el código fuera del "top-level" y dentro de un método.
    public static async Task Main(string[] args)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var builder = WebApplication.CreateBuilder(args);

        #region DATABASE
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Único registro global de HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
        });

        // Inyección de servicios personalizados (Multisite Content & Tours)
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddHttpClient<IEmailService, ResendEmailService>();
        builder.Services.AddScoped<ISiteService, SiteService>();
        builder.Services.AddScoped<IContentService, ContentService>();
        builder.Services.AddScoped<ITourService, TourService>();
        builder.Services.AddScoped<IMediaService, MediaService>();
        builder.Services.AddScoped<IPageService, PageService>();
        #endregion

        #region JWT AUTHENTICATION
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        var jwtKey = jwtSection["Key"];

        Console.WriteLine($"DEBUG: La clave leída es: {jwtKey}");

        // Evitamos que tire la app por completo si estás probando vistas sin auth. Ponemos un fallback seguro.
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            Console.WriteLine("WARNING: JwtSettings:Key no configurado. Utilizando clave de emergencia para desarrollo.");
            jwtKey = "Clave_Super_Secreta_Y_Larga_De_Emergencia_Para_CMS_2026";
        }

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        var key = new SymmetricSecurityKey(keyBytes);
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
                    ValidIssuer = jwtSection["Issuer"] ?? "CMSApi",
                    ValidAudience = jwtSection["Audience"] ?? "FrontendApp",
                    IssuerSigningKey = key,
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
                    .WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:4321",
                        "https://romantic-spence.74-208-70-235.plesk.page",
                        "https://reverent-knuth.74-208-70-235.plesk.page"
                    )
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "Accept");
            });
        });
        #endregion

        #region CONTROLLERS & SWAGGER
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS API", Version = "v1" });
            c.CustomSchemaIds(type => type.FullName);
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

        #region MIDDLEWARE PIPELINE
        app.UseCors("AllowFrontend");
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        #endregion

        await app.RunAsync();
    }
}