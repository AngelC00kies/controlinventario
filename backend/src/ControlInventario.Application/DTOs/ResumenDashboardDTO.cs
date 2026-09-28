namespace ControlInventario.Application.DTOs;

/// <summary>Resumen general para el dashboard.</summary>
public class ResumenDashboardDTO
{
    public int TotalProductos { get; set; }

    public int TotalAlmacenes { get; set; }

    public int TotalProveedores { get; set; }

    /// <summary>Cantidad de movimientos registrados en el mes en curso.</summary>
    public int MovimientosMes { get; set; }

    /// <summary>Valor total del inventario al precio de compra.</summary>
    public decimal ValorInventario { get; set; }

    /// <summary>Cantidad de productos con stock crítico.</summary>
    public int ProductosStockCritico { get; set; }

    /// <summary>Alertas de stock bajo (máx. 5).</summary>
    public List<AlertaStockDTO> Alertas { get; set; } = new();

    /// <summary>Últimos movimientos registrados (máx. 5).</summary>
    public List<MovimientoDTO> UltimosMovimientos { get; set; } = new();
}
