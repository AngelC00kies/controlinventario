using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInventario.API.Controllers;

/// <summary>
/// Indicadores del dashboard (totales, valor de inventario, alertas y últimos movimientos).
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IServicioDashboard _servicio;

    public DashboardController(IServicioDashboard servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Resumen general del sistema.</summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenDashboardDTO>> Resumen()
    {
        return Ok(await _servicio.ResumenAsync());
    }
}
