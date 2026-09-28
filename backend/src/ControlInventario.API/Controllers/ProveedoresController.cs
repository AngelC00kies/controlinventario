using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// CRUD de proveedores con sus datos de contacto.
/// </summary>
[ApiController]
[Route("api/proveedores")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly IServicioProveedores _servicio;

    public ProveedoresController(IServicioProveedores servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista los proveedores.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProveedorDTO>>> Listar([FromQuery] bool incluirInactivos = false)
    {
        return Ok(await _servicio.ListarAsync(soloActivos: !incluirInactivos));
    }

    /// <summary>Obtiene un proveedor por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProveedorDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>Alta de proveedor (solo Administrador).</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProveedorDTO>> Crear([FromBody] ProveedorCreacionDTO dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdProveedor }, creado);
    }

    /// <summary>Modificación de proveedor (solo Administrador).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProveedorDTO>> Actualizar(int id, [FromBody] ProveedorActualizacionDTO dto)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto));
    }

    /// <summary>Baja lógica de proveedor (solo Administrador).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
