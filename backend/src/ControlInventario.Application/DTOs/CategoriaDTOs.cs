namespace ControlInventario.Application.DTOs;

/// <summary>Datos de una categoría.</summary>
public class CategoriaDTO
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    /// <summary>Cantidad de productos asociados (informativo).</summary>
    public int TotalProductos { get; set; }
}

/// <summary>Datos para crear una categoría.</summary>
public class CategoriaCreacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
}

/// <summary>Datos para modificar una categoría.</summary>
public class CategoriaActualizacionDTO
{
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}
