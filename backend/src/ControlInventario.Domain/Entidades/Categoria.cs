namespace ControlInventario.Domain.Entidades;

/// <summary>
/// Categoría a la que pertenecen los productos.
/// </summary>
public class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>Productos que pertenecen a esta categoría.</summary>
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
