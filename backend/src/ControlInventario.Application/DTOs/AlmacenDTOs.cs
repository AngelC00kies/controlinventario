namespace ControlInventario.Application.DTOs;

/// <summary>Datos de un almacén.</summary>
public class AlmacenDTO
{
    public int IdAlmacen { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Ubicacion { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}

/// <summary>Datos para crear un almacén.</summary>
public class AlmacenCreacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Ubicacion { get; set; }

    public string? Descripcion { get; set; }
}

/// <summary>Datos para modificar un almacén.</summary>
public class AlmacenActualizacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Ubicacion { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
