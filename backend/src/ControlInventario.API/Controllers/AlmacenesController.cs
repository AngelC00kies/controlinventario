using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// CRUD de almacenes (el sistema soporta uno o varios almacenes).
/// </summary>
[ApiController]
[Route("api/almacenes")]
[Authorize]
public class AlmacenesController : ControllerBase
{
    private readonly IServicioAlmacenes _servicio;

    public AlmacenesController(IServicioAlmacenes servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista los almacenes.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AlmacenDTO>>> Listar([FromQuery] bool incluirInactivos = false)
    {
        return Ok(await _servicio.ListarAsync(soloActivos: !incluirInactivos));
    }

    /// <summary>Obtiene un almacén por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlmacenDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>Alta de almacén (solo Administrador).</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<AlmacenDTO>> Crear([FromBody] AlmacenCreacionDTO dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdAlmacen }, creado);
    }

    /// <summary>Modificación de almacén (solo Administrador).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<AlmacenDTO>> Actualizar(int id, [FromBody] AlmacenActualizacionDTO dto)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto));
    }

    /// <summary>Baja lógica de almacén (solo Administrador).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
