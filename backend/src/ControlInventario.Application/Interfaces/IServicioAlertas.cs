using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de alertas de stock bajo.</summary>
public interface IServicioAlertas
{
    /// <summary>Productos con stock total menor o igual al stock mínimo.</summary>
    Task<IReadOnlyList<AlertaStockDTO>> ObtenerStockBajoAsync();
}
