using ControlInventario.Domain.Enums;

namespace ControlInventario.Application.DTOs;

/// <summary>Línea de un movimiento (detalle).</summary>
public class MovimientoDetalleDTO
{
    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string NombreProducto { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    /// <summary>Cantidad × PrecioUnitario.</summary>
    public decimal Subtotal { get; set; }
}

/// <summary>Movimiento completo con sus detalles y datos de contexto.</summary>
public class MovimientoDTO
{
    public int IdMovimiento { get; set; }

    public DateTime Fecha { get; set; }

    /// <summary>"Entrada" o "Salida".</summary>
    public string Tipo { get; set; } = string.Empty;

    public string? Motivo { get; set; }

    public string? DocumentoReferencia { get; set; }

    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string NombreAlmacen { get; set; } = string.Empty;

    public int? IdProveedor { get; set; }

    public string? NombreProveedor { get; set; }

    /// <summary>Suma de los subtotales de sus detalles.</summary>
    public decimal Total { get; set; }

    public List<MovimientoDetalleDTO> Detalles { get; set; } = new();
}

/// <summary>Línea para crear un movimiento.</summary>
public class MovimientoDetalleCreacionDTO
{
    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    /// <summary>Precio unitario registrado. Si es 0 se usa el precio por defecto del producto.</summary>
    public decimal PrecioUnitario { get; set; }
}

/// <summary>Datos para registrar un movimiento (entrada o salida).</summary>
public class MovimientoCreacionDTO
{
    public TipoMovimiento Tipo { get; set; }

    public string? Motivo { get; set; }

    /// <summary>Número de factura u otro documento asociado.</summary>
    public string? DocumentoReferencia { get; set; }

    public int IdAlmacen { get; set; }

    /// <summary>Proveedor asociado (opcional, típico en entradas por compra).</summary>
    public int? IdProveedor { get; set; }

    public List<MovimientoDetalleCreacionDTO> Detalles { get; set; } = new();
}

/// <summary>Filtros opcionales para buscar movimientos.</summary>
public class FiltroMovimientoDTO
{
    public int? IdProducto { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public int? IdUsuario { get; set; }

    public TipoMovimiento? Tipo { get; set; }
}
