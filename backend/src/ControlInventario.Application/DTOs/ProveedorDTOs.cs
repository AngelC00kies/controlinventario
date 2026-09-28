namespace ControlInventario.Application.DTOs;

/// <summary>Datos de un proveedor con su información de contacto.</summary>
public class ProveedorDTO
{
    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public bool Activo { get; set; }

    /// <summary>Cantidad de productos asociados (informativo).</summary>
    public int TotalProductos { get; set; }
}

/// <summary>Datos para crear un proveedor.</summary>
public class ProveedorCreacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }
}

/// <summary>Datos para modificar un proveedor.</summary>
public class ProveedorActualizacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Contacto { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public bool Activo { get; set; }
}
