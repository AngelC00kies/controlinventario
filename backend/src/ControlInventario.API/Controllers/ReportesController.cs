using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Reportes del inventario: stock actual, movimientos por producto/fecha/usuario
/// y productos con stock crítico.
/// </summary>
[ApiController]
[Route("api/reportes")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IServicioReportes _servicio;

    public ReportesController(IServicioReportes servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Reporte de stock actual (opcional: filtrar por categoría y solo críticos).</summary>
    [HttpGet("stock-actual")]
    public async Task<ActionResult<IReadOnlyList<StockActualDTO>>> StockActual(
        [FromQuery] int? idCategoria,
        [FromQuery] bool soloCriticos = false)
    {
        return Ok(await _servicio.StockActualAsync(idCategoria, soloCriticos));
    }

    /// <summary>Reporte de movimientos por producto, rango de fechas y/o usuario.</summary>
    [HttpGet("movimientos")]
    public async Task<ActionResult<IReadOnlyList<MovimientoReporteDTO>>> Movimientos(
        [FromQuery] int? idProducto,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? idUsuario,
        [FromQuery] TipoMovimiento? tipo)
    {
        var filas = await _servicio.MovimientosAsync(new FiltroMovimientoDTO
        {
            IdProducto = idProducto,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            IdUsuario = idUsuario,
            Tipo = tipo
        });

        return Ok(filas);
    }

    /// <summary>Reporte de productos con stock crítico.</summary>
    [HttpGet("stock-critico")]
    public async Task<ActionResult<IReadOnlyList<AlertaStockDTO>>> StockCritico()
    {
        return Ok(await _servicio.StockCriticoAsync());
    }
}
