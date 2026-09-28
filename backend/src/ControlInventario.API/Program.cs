using System.Text;
using System.Text.Json.Serialization;
using ControlInventario.Application;
using ControlInventario.Application.Comun;
using ControlInventario.API.Middleware;
using ControlInventario.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var constructor = WebApplication.CreateBuilder(args);

/* -------------------------------------------------------------------------
   1. JSON: camelCase (convención REST) y enums como texto (Entrada/Salida).
   ------------------------------------------------------------------------- */
constructor.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

/* -------------------------------------------------------------------------
   2. Capas de la arquitectura: Application (servicios) e Infrastructure
      (repositorios y base de datos). Inversión de dependencias.
   ------------------------------------------------------------------------- */
constructor.Services.AgregarAplicacion();

var cadenaConexion = constructor.Configuration.GetConnectionString("Principal")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Principal' en appsettings.json.");

constructor.Services.AgregarInfraestructura(cadenaConexion);

/* -------------------------------------------------------------------------
   3. Autenticación JWT.
   ------------------------------------------------------------------------- */
var opcionesJwt = constructor.Configuration.GetSection("Jwt").Get<OpcionesJwt>()
    ?? throw new InvalidOperationException("Falta la sección 'Jwt' en appsettings.json.");

if (string.IsNullOrWhiteSpace(opcionesJwt.ClaveSecreta) || opcionesJwt.ClaveSecreta.Length < 32)
{
    throw new InvalidOperationException("La clave secreta JWT debe tener al menos 32 caracteres.");
}

constructor.Services.Configure<OpcionesJwt>(constructor.Configuration.GetSection("Jwt"));

constructor.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        // Los claims se emiten con nombres propios ("sub", "rol", ...): no se
        // mapean a los tipos URI por defecto, definiendo los nombres de lectura.
        opciones.MapInboundClaims = false;
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = opcionesJwt.Emisor,
            ValidateAudience = true,
            ValidAudience = opcionesJwt.Audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcionesJwt.ClaveSecreta)),
            NameClaimType = "unique_name",
            RoleClaimType = "rol"
        };
    });

constructor.Services.AddAuthorization();

/* -------------------------------------------------------------------------
   4. CORS: permite el frontend de desarrollo (Vite) en localhost:5173.
   ------------------------------------------------------------------------- */
constructor.Services.AddCors(opciones =>
{
    opciones.AddPolicy("Frontend", politica => politica
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

/* -------------------------------------------------------------------------
   5. Swagger con soporte de token Bearer.
   ------------------------------------------------------------------------- */
constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ControlInventario API",
        Version = "v1",
        Description = "API del sistema de inventario ControlInventario (entradas, salidas, stock y reportes)."
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT devuelto por /api/auth/login."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var aplicacion = constructor.Build();

/* -------------------------------------------------------------------------
   6. Pipeline de la aplicación.
   ------------------------------------------------------------------------- */
if (aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseSwagger();
    aplicacion.UseSwaggerUI();
}

// Middleware global de excepciones (traduce ExcepcionNegocio a HTTP 400, etc.).
aplicacion.UseMiddleware<MiddlewareExcepciones>();

aplicacion.UseCors("Frontend");
aplicacion.UseAuthentication();
aplicacion.UseAuthorization();
aplicacion.MapControllers();

aplicacion.Run();
