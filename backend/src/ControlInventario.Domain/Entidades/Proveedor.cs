namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Proveedor de productos, con sus datos de contacto.
/// </summary>
public class Proveedor
{
    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>Productos que suministra este proveedor.</summary>
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();

    /// <summary>Movimientos de entrada asociados a este proveedor.</summary>
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
