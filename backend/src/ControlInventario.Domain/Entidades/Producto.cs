namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Producto del inventario. El stock actual NO se guarda aquí sino en
/// <see cref="StockPorAlmacen"/> (stock por cada almacén).
/// </summary>
public class Producto
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int IdCategoria { get; set; }

    /// <summary>Categoría del producto.</summary>
    public Categoria? Categoria { get; set; }

    public int IdProveedor { get; set; }

    /// <summary>Proveedor del producto.</summary>
    public Proveedor? Proveedor { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public string UnidadMedida { get; set; } = "Unidad";

    /// <summary>Stock mínimo; si el stock total iguala o baja este valor se dispara la alerta.</summary>
    public int StockMinimo { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>Cantidad de este producto en cada almacén.</summary>
    public ICollection<StockPorAlmacen> StockPorAlmacen { get; set; } = new List<StockPorAlmacen>();

    /// <summary>Líneas de movimiento que involucran este producto.</summary>
    public ICollection<MovimientoDetalle> MovimientoDetalles { get; set; } = new List<MovimientoDetalle>();
}
