using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Alertas de stock bajo (productos con stock menor o igual al mínimo).
/// </summary>
[ApiController]
[Route("api/alertas")]
[Authorize]
public class AlertasController : ControllerBase
{
    private readonly IServicioAlertas _servicio;

    public AlertasController(IServicioAlertas servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Devuelve los productos con stock crítico, ordenados por urgencia.</summary>
    [HttpGet("stock-bajo")]
    public async Task<ActionResult<IReadOnlyList<AlertaStockDTO>>> StockBajo()
    {
        return Ok(await _servicio.ObtenerStockBajoAsync());
    }
}
