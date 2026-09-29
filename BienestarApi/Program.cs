using System.Text;
using BienestarApi.Data;
using BienestarApi.Models;
using BienestarApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios ----------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// EF Core + SQL Server. La cadena de conexión vive en appsettings.json
// (ConnectionStrings:DefaultConnection) para no hardcodear credenciales.
builder.Services.AddDbContext<BienestarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticación (Sprint 2): hashing de contraseñas (PBKDF2, el mismo
// algoritmo probado que usa ASP.NET Core Identity, sin traer todo Identity)
// y los servicios propios de token/login.
builder.Services.AddScoped<IPasswordHasher<Cuenta>, PasswordHasher<Cuenta>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Sprint 3: check-in diario y encuesta basal.
builder.Services.AddSingleton<IRelojPiloto, RelojPiloto>();
builder.Services.AddScoped<IRegistroDiarioService, RegistroDiarioService>();
builder.Services.AddScoped<IEncuestaBasalService, EncuestaBasalService>();
builder.Services.AddScoped<IEncuestaSusService, EncuestaSusService>();

// Autenticación JWT: se configura el esquema de validación de tokens desde
// ya (Sprint 1), aunque el endpoint de login que EMITE el token se
// implementa recién en el Sprint 2. Así el pipeline de middlewares y el
// atributo [Authorize] ya quedan listos para usarse sin tocar Program.cs.
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Key en appsettings.json");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ---------- Pipeline HTTP ----------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Orden importante: primero autenticación (¿quién eres?), luego
// autorización (¿qué puedes hacer?).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
