using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio de autenticación: valida credenciales con el PasswordHasher de
/// ASP.NET Core Identity y emite tokens JWT firmados.
/// </summary>
public class ServicioAuth : IServicioAuth
{
    private readonly IRepositorioUsuarios _usuarios;
    private readonly IPasswordHasher<Domain.Entidades.Usuario> _hasherContrasenas;
    private readonly IOptions<OpcionesJwt> _opciones;
    private readonly IValidator<SolicitudLoginDTO> _validadorLogin;

    public ServicioAuth(
        IRepositorioUsuarios usuarios,
        IPasswordHasher<Domain.Entidades.Usuario> hasherContrasenas,
        IOptions<OpcionesJwt> opciones,
        IValidator<SolicitudLoginDTO> validadorLogin)
    {
        _usuarios = usuarios;
        _hasherContrasenas = hasherContrasenas;
        _opciones = opciones;
        _validadorLogin = validadorLogin;
    }

    /// <inheritdoc />
    public async Task<RespuestaAuthDTO> IniciarSesionAsync(SolicitudLoginDTO solicitud)
    {
        var validacion = await _validadorLogin.ValidateAsync(solicitud);
        if (!validacion.IsValid)
        {
            throw new ExcepcionNegocio(string.Join(" ", validacion.Errors.Select(e => e.ErrorMessage)));
        }

        var usuario = await _usuarios.ObtenerPorNombreUsuarioAsync(solicitud.NombreUsuario);

        // Mensaje genérico para no revelar si el usuario existe o no.
        if (usuario is null)
        {
            throw new ExcepcionNoAutorizado("Usuario o contraseña incorrectos.");
        }

        var resultado = _hasherContrasenas.VerifyHashedPassword(usuario, usuario.ContrasenaHash, solicitud.Contrasena);
        if (resultado == PasswordVerificationResult.Failed)
        {
            throw new ExcepcionNoAutorizado("Usuario o contraseña incorrectos.");
        }

        if (!usuario.Activo)
        {
            throw new ExcepcionNoAutorizado("El usuario está inactivo. Contacte al administrador.");
        }

        // Si el hash quedó con un algoritmo anterior, se re-eleva automáticamente.
        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.ContrasenaHash = _hasherContrasenas.HashPassword(usuario, solicitud.Contrasena);
            _usuarios.Actualizar(usuario);
        }

        return GenerarRespuesta(usuario);
    }

    /// <inheritdoc />
    public async Task<RespuestaAuthDTO> ObtenerPerfilAsync(int idUsuario)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(idUsuario)
            ?? throw new ExcepcionNoEncontrado("Usuario no encontrado.");

        return GenerarRespuesta(usuario);
    }

    /// <summary>Construye el token JWT y la respuesta con los datos del usuario.</summary>
    private RespuestaAuthDTO GenerarRespuesta(Domain.Entidades.Usuario usuario)
    {
        var opciones = _opciones.Value;
        var expiracion = DateTime.UtcNow.AddMinutes(opciones.DuracionMinutos);

        var claims = new List<Claim>
        {
            new("sub", usuario.IdUsuario.ToString()),
            new("unique_name", usuario.NombreUsuario),
            new("email", usuario.Correo),
            new("nombre_completo", usuario.NombreCompleto),
            new("rol", usuario.Rol?.Nombre ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opciones.ClaveSecreta));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: opciones.Emisor,
            audience: opciones.Audiencia,
            claims: claims,
            expires: expiracion,
            signingCredentials: credenciales);

        return new RespuestaAuthDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo,
            Rol = usuario.Rol?.Nombre ?? string.Empty,
            ExpiraEn = expiracion.ToLocalTime()
        };
    }
}
