using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de indicadores del dashboard.</summary>
public interface IServicioDashboard
{
    /// <summary>Resumen general: totales, valor de inventario, alertas y últimos movimientos.</summary>
    Task<ResumenDashboardDTO> ResumenAsync();
}
