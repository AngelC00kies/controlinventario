namespace ControlInventario.Domain.Enums;

/// <summary>
/// Tipos de movimiento de inventario.
/// Los valores coinciden con el CHECK de la tabla dbo.Movimientos en español.
/// </summary>
public enum TipoMovimiento
{
    /// <summary>Ingreso de mercadería al almacén (aumenta el stock).</summary>
    Entrada = 1,

    /// <summary>Egreso de mercadería del almacén (disminuye el stock).</summary>
    Salida = 2
}
