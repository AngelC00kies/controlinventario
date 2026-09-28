using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de reportes y consultas del inventario.</summary>
public interface IServicioReportes
{
    /// <summary>Reporte de stock actual, con filtros opcionales de categoría y solo críticos.</summary>
    Task<IReadOnlyList<StockActualDTO>> StockActualAsync(int? idCategoria, bool soloCriticos);

    /// <summary>Reporte de movimientos por producto, rango de fechas y/o usuario.</summary>
    Task<IReadOnlyList<MovimientoReporteDTO>> MovimientosAsync(FiltroMovimientoDTO filtros);

    /// <summary>Reporte de productos con stock crítico.</summary>
    Task<IReadOnlyList<AlertaStockDTO>> StockCriticoAsync();
}
