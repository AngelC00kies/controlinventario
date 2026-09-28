using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Administración de usuarios y roles. Acceso exclusivo del rol Administrador.
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly IServicioUsuarios _servicio;

    public UsuariosController(IServicioUsuarios servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista los usuarios con su rol.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDTO>>> Listar()
    {
        return Ok(await _servicio.ListarAsync());
    }

    /// <summary>Lista los roles disponibles (para el formulario de usuarios).</summary>
    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<RolDTO>>> ObtenerRoles()
    {
        return Ok(await _servicio.ObtenerRolesAsync());
    }

    /// <summary>Obtiene un usuario por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>Alta de usuario.</summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> Crear([FromBody] UsuarioCreacionDTO dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdUsuario }, creado);
    }

    /// <summary>Modificación de usuario (la contraseña es opcional).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioDTO>> Actualizar(int id, [FromBody] UsuarioActualizacionDTO dto)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto));
    }

    /// <summary>Baja lógica de usuario.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
