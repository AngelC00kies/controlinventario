namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Línea (detalle) de un movimiento: producto, cantidad y precio unitario.
/// </summary>
public class MovimientoDetalle
{
    public int IdMovimientoDetalle { get; set; }

    public int IdMovimiento { get; set; }

    /// <summary>Movimiento al que pertenece esta línea.</summary>
    public Movimiento? Movimiento { get; set; }

    public int IdProducto { get; set; }

    /// <summary>Producto afectado por esta línea.</summary>
    public Producto? Producto { get; set; }

    /// <summary>Cantidad movida. Siempre mayor que cero.</summary>
    public int Cantidad { get; set; }

    /// <summary>Precio unitario registrado al momento del movimiento.</summary>
    public decimal PrecioUnitario { get; set; }
}
