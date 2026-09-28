namespace ControlInventario.Application.DTOs;

/// <summary>Datos completos de un producto, incluyendo stock calculado.</summary>
public class ProductoDTO
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int IdCategoria { get; set; }

    public string NombreCategoria { get; set; } = string.Empty;

    public int IdProveedor { get; set; }

    public string NombreProveedor { get; set; } = string.Empty;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public int StockMinimo { get; set; }

    /// <summary>Stock total actual (suma de todos los almacenes).</summary>
    public int StockActual { get; set; }

    /// <summary>true cuando StockActual es menor o igual al StockMinimo.</summary>
    public bool StockCritico { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}

/// <summary>Datos para crear un producto.</summary>
public class ProductoCreacionDTO
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int IdCategoria { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public string UnidadMedida { get; set; } = "Unidad";

    public int StockMinimo { get; set; }
}

/// <summary>Datos para modificar un producto.</summary>
public class ProductoActualizacionDTO
{
    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int IdCategoria { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public string UnidadMedida { get; set; } = "Unidad";

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }
}

/// <summary>Stock de un producto en un almacén puntual.</summary>
public class StockProductoDTO
{
    public int IdAlmacen { get; set; }

    public string NombreAlmacen { get; set; } = string.Empty;

    public int Cantidad { get; set; }
}
