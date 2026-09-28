using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Gestión de productos: consulta para todos los roles autenticados y
/// altas/modificaciones/bajas solo para Administrador.
/// </summary>
[ApiController]
[Route("api/productos")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IServicioProductos _servicio;

    public ProductosController(IServicioProductos servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista productos con filtros opcionales de búsqueda, categoría y proveedor.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductoDTO>>> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? idCategoria,
        [FromQuery] int? idProveedor,
        [FromQuery] bool incluirInactivos = false)
    {
        var productos = await _servicio.ListarAsync(busqueda, idCategoria, idProveedor, soloActivos: !incluirInactivos);
        return Ok(productos);
    }

    /// <summary>Obtiene un producto por su identificador.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>Stock del producto desglosado por almacén.</summary>
    [HttpGet("{id:int}/stock")]
    public async Task<ActionResult<IReadOnlyList<StockProductoDTO>>> ObtenerStock(int id)
    {
        return Ok(await _servicio.ObtenerStockAsync(id));
    }

    /// <summary>Alta de producto (solo Administrador).</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoDTO>> Crear([FromBody] ProductoCreacionDTO dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdProducto }, creado);
    }

    /// <summary>Modificación de producto (solo Administrador).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoDTO>> Actualizar(int id, [FromBody] ProductoActualizacionDTO dto)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto));
    }

    /// <summary>Baja lógica de producto (solo Administrador).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
