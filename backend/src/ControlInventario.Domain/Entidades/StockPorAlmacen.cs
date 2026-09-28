namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Cantidad actual de un producto dentro de un almacén.
/// Relación (Producto, Almacen) única.
/// </summary>
public class StockPorAlmacen
{
    public int IdStock { get; set; }

    public int IdProducto { get; set; }

    /// <summary>Producto al que pertenece esta cantidad.</summary>
    public Producto? Producto { get; set; }

    public int IdAlmacen { get; set; }

    /// <summary>Almacén donde se encuentra la cantidad.</summary>
    public Almacen? Almacen { get; set; }

    /// <summary>Cantidad disponible. Nunca puede ser negativa.</summary>
    public int Cantidad { get; set; }
}
