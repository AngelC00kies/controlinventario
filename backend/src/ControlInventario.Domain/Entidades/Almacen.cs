namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Almacén o bodega donde se resguarda el stock de productos.
/// El sistema soporta uno o varios almacenes.
/// </summary>
public class Almacen
{
    public int IdAlmacen { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Ubicacion { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>Stock de productos almacenado en este almacén.</summary>
    public ICollection<StockPorAlmacen> StockPorAlmacen { get; set; } = new List<StockPorAlmacen>();

    /// <summary>Movimientos realizados en este almacén.</summary>
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
