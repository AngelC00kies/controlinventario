using ControlInventario.Domain.Enums;

namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Encabezado de un movimiento de inventario: fecha, tipo (entrada/salida),
/// motivo, usuario responsable, almacén y documento/factura asociada.
/// </summary>
public class Movimiento
{
    public int IdMovimiento { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    /// <summary>Tipo de movimiento: Entrada o Salida.</summary>
    public TipoMovimiento Tipo { get; set; }

    public string? Motivo { get; set; }

    /// <summary>Número de factura u otro documento asociado al movimiento.</summary>
    public string? DocumentoReferencia { get; set; }

    public int IdUsuario { get; set; }

    /// <summary>Usuario responsable del movimiento.</summary>
    public Usuario? Usuario { get; set; }

    public int IdAlmacen { get; set; }

    /// <summary>Almacén donde entra o sale la mercadería.</summary>
    public Almacen? Almacen { get; set; }

    public int? IdProveedor { get; set; }

    /// <summary>Proveedor asociado (típico en entradas por compra).</summary>
    public Proveedor? Proveedor { get; set; }

    /// <summary>Líneas del movimiento (productos y cantidades).</summary>
    public ICollection<MovimientoDetalle> Detalles { get; set; } = new List<MovimientoDetalle>();
}
