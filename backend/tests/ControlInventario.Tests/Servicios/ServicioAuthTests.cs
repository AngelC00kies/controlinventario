using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Servicios;
using ControlInventario.Application.Validadores;
using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Tests.Config;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace ControlInventario.Tests.Servicios;

/// <summary>
/// Pruebas unitarias del ServicioAuth: credenciales, usuarios inactivos,
/// emisión del token y verificación del hash semilla de SQL Server.
/// </summary>
public class ServicioAuthTests
{
    private const string ClavePrueba = "esta-es-una-clave-secreta-jwt-de-prueba-123456";

    private readonly Mock<IRepositorioUsuarios> _usuarios = new();
    private readonly ServicioAuth _servicio;
    private readonly PasswordHasher<Usuario> _hasher = new();

    public ServicioAuthTests()
    {
        _servicio = new ServicioAuth(
            _usuarios.Object,
            _hasher,
            Options.Create(new OpcionesJwt
            {
                ClaveSecreta = ClavePrueba,
                Emisor = "ControlInventario",
                Audiencia = "ControlInventarioFrontend",
                DuracionMinutos = 60
            }),
            new SolicitudLoginValidador());
    }

    private Usuario UsuarioConContrasena(string contrasena = "Admin123*")
    {
        var usuario = DatosPrueba.Usuario();
        usuario.ContrasenaHash = _hasher.HashPassword(usuario, contrasena);
        return usuario;
    }

    [Fact]
    public async Task IniciarSesion_ConCredencialesValidas_DeberiaDevolverToken()
    {
        var usuario = UsuarioConContrasena();
        usuario.Rol!.Nombre = "Administrador";
        _usuarios.Setup(u => u.ObtenerPorNombreUsuarioAsync("admin")).ReturnsAsync(usuario);

        var resultado = await _servicio.IniciarSesionAsync(new SolicitudLoginDTO
        {
            NombreUsuario = "admin",
            Contrasena = "Admin123*"
        });

        resultado.Token.Should().NotBeNullOrWhiteSpace();
        resultado.Rol.Should().Be("Administrador");
        resultado.NombreUsuario.Should().Be("admin");
        resultado.ExpiraEn.Should().BeAfter(DateTime.Now);
    }

    [Fact]
    public async Task IniciarSesion_ConContrasenaIncorrecta_DeberiaLanzarNoAutorizado()
    {
        var usuario = UsuarioConContrasena();
        _usuarios.Setup(u => u.ObtenerPorNombreUsuarioAsync("admin")).ReturnsAsync(usuario);

        var accion = async () => await _servicio.IniciarSesionAsync(new SolicitudLoginDTO
        {
            NombreUsuario = "admin",
            Contrasena = "contrasena-incorrecta"
        });

        await accion.Should().ThrowAsync<ExcepcionNoAutorizado>();
    }

    [Fact]
    public async Task IniciarSesion_UsuarioInexistente_DeberiaLanzarNoAutorizado()
    {
        _usuarios.Setup(u => u.ObtenerPorNombreUsuarioAsync("nadie")).ReturnsAsync((Usuario?)null);

        var accion = async () => await _servicio.IniciarSesionAsync(new SolicitudLoginDTO
        {
            NombreUsuario = "nadie",
            Contrasena = "cualquiera123"
        });

        await accion.Should().ThrowAsync<ExcepcionNoAutorizado>();
    }

    [Fact]
    public async Task IniciarSesion_UsuarioInactivo_DeberiaLanzarNoAutorizado()
    {
        var usuario = UsuarioConContrasena();
        usuario.Activo = false;
        _usuarios.Setup(u => u.ObtenerPorNombreUsuarioAsync("admin")).ReturnsAsync(usuario);

        var accion = async () => await _servicio.IniciarSesionAsync(new SolicitudLoginDTO
        {
            NombreUsuario = "admin",
            Contrasena = "Admin123*"
        });

        await accion.Should().ThrowAsync<ExcepcionNoAutorizado>()
            .WithMessage("*inactivo*");
    }

    [Fact]
    public async Task IniciarSesion_SinUsuario_DeberiaLanzarExcepcionDeNegocio()
    {
        var accion = async () => await _servicio.IniciarSesionAsync(new SolicitudLoginDTO
        {
            NombreUsuario = "",
            Contrasena = ""
        });

        await accion.Should().ThrowAsync<ExcepcionNegocio>();
    }

    /// <summary>
    /// Garantiza que el hash semillado en database/ControlInventario.sql
    /// (usuario admin / Admin123*) puede verificarse con PasswordHasher de
    /// ASP.NET Core Identity.
    /// </summary>
    [Fact]
    public void HashSemillaDelScriptSQL_DeberiaVerificarseConPasswordHasher()
    {
        // Mismo valor insertado por el script SQL para el usuario "admin".
        const string hashSemilla =
            "AQAAAAIAAYagAAAAEDyna07dG+aQrOcrFhvXLRHqHea7Lm2hN8pMdsA1gsnKagwVg60Xhg4oO9e2psD7Mw==";

        var usuario = DatosPrueba.Usuario();
        var resultado = _hasher.VerifyHashedPassword(usuario, hashSemilla, "Admin123*");

        resultado.Should().Be(PasswordVerificationResult.Success);
    }

    /// <summary>
    /// Un hash generado con un algoritmo anterior (HMAC-SHA256) también debe
    /// aceptarse: Identity devuelve SuccessRehashNeeded y el ServicioAuth
    /// re-eleva el hash automáticamente en el primer inicio de sesión.
    /// </summary>
    [Fact]
    public void HashAntiguoHmacSha256_DeberiaAcepetarseYRehashearse()
    {
        const string hashSha256 =
            "AQAAAAEAAYagAAAAEF42qHpE1PPbGi9BaiYk4FPnCdNfpplhojU7BE1nABWcoYeDcjZIIEwTOslZUaGWoA==";

        var usuario = DatosPrueba.Usuario();
        var resultado = _hasher.VerifyHashedPassword(usuario, hashSha256, "Admin123*");

        resultado.Should().Be(PasswordVerificationResult.SuccessRehashNeeded);
    }
}
