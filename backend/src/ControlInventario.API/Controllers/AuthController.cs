using ControlInventario.API.Utilidades;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Endpoints de autenticación: inicio de sesión y perfil del usuario.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IServicioAuth _servicioAuth;

    public AuthController(IServicioAuth servicioAuth)
    {
        _servicioAuth = servicioAuth;
    }

    /// <summary>Valida credenciales y devuelve el token JWT (público).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaAuthDTO>> IniciarSesion([FromBody] SolicitudLoginDTO solicitud)
    {
        return Ok(await _servicioAuth.IniciarSesionAsync(solicitud));
    }

    /// <summary>Devuelve el perfil del usuario autenticado (token vigente).</summary>
    [HttpGet("yo")]
    [Authorize]
    public async Task<ActionResult<RespuestaAuthDTO>> ObtenerPerfil()
    {
        return Ok(await _servicioAuth.ObtenerPerfilAsync(UtilidadesClaims.ObtenerIdUsuario(User)));
    }
}
