using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// CRUD de categorías de productos.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly IServicioCategorias _servicio;

    public CategoriasController(IServicioCategorias servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista las categorías.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaDTO>>> Listar([FromQuery] bool incluirInactivas = false)
    {
        return Ok(await _servicio.ListarAsync(soloActivas: !incluirInactivas));
    }

    /// <summary>Obtiene una categoría por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>Alta de categoría (solo Administrador).</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDTO>> Crear([FromBody] CategoriaCreacionDTO dto)
    {
        var creada = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdCategoria }, creada);
    }

    /// <summary>Modificación de categoría (solo Administrador).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDTO>> Actualizar(int id, [FromBody] CategoriaActualizacionDTO dto)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto));
    }

    /// <summary>Baja lógica de categoría (solo Administrador).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
