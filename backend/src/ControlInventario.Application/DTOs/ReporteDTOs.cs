namespace ControlInventario.Application.DTOs;

/// <summary>Fila del reporte de stock actual.</summary>
public class StockActualDTO
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Proveedor { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    public int StockActual { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    /// <summary>StockActual × PrecioCompra.</summary>
    public decimal ValorInventario { get; set; }

    /// <summary>"Crítico" cuando StockActual ≤ StockMinimo, si no "Normal".</summary>
    public string Estado { get; set; } = string.Empty;
}

/// <summary>Fila del reporte de movimientos (una por línea de detalle).</summary>
public class MovimientoReporteDTO
{
    public int IdMovimiento { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public string? Motivo { get; set; }

    public string? DocumentoReferencia { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Almacen { get; set; } = string.Empty;

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
}
