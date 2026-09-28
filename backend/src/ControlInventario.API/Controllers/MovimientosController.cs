using ControlInventario.API.Utilidades;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Registro y consulta de movimientos de inventario (entradas y salidas).
/// </summary>
[ApiController]
[Route("api/movimientos")]
[Authorize]
public class MovimientosController : ControllerBase
{
    private readonly IServicioMovimientos _servicio;

    public MovimientosController(IServicioMovimientos servicio)
    {
        _servicio = servicio;
    }

    /// <summary>
    /// Busca movimientos. Filtros opcionales: producto, fechas, usuario y tipo
    /// (Entrada/Salida).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MovimientoDTO>>> Buscar(
        [FromQuery] int? idProducto,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? idUsuario,
        [FromQuery] TipoMovimiento? tipo)
    {
        var movimientos = await _servicio.BuscarAsync(new FiltroMovimientoDTO
        {
            IdProducto = idProducto,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            IdUsuario = idUsuario,
            Tipo = tipo
        });

        return Ok(movimientos);
    }

    /// <summary>Obtiene un movimiento con sus detalles.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovimientoDTO>> ObtenerPorId(int id)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id));
    }

    /// <summary>
    /// Registra un movimiento de entrada o salida y actualiza el stock
    /// automáticamente. Permitido para Administrador y Usuario.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador,Usuario")]
    public async Task<ActionResult<MovimientoDTO>> Crear([FromBody] MovimientoCreacionDTO dto)
    {
        var idUsuario = UtilidadesClaims.ObtenerIdUsuario(User);
        var creado = await _servicio.CrearAsync(dto, idUsuario);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdMovimiento }, creado);
    }
}
