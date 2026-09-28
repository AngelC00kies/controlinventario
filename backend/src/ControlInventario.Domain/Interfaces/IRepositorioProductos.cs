using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>
/// Contrato del repositorio de productos con las consultas específicas del inventario.
/// </summary>
public interface IRepositorioProductos : IRepositorio<Producto>
{
    /// <summary>Indica si ya existe un producto activo con el código indicado.</summary>
    Task<bool> ExisteCodigoAsync(string codigo, int? idProductoExcluir = null);

    /// <summary>
    /// Busca productos aplicando filtros opcionales (código, nombre, categoría, proveedor).
    /// Incluye categoría, proveedor y stock total calculado.
    /// </summary>
    Task<IReadOnlyList<Producto>> BuscarAsync(string? busqueda, int? idCategoria, int? idProveedor, bool soloActivos = true);

    /// <summary>Obtiene un producto por id incluyendo categoría, proveedor y stock por almacén.</summary>
    Task<Producto?> ObtenerCompletoAsync(int id);

    /// <summary>Suma del stock de un producto en todos los almacenes.</summary>
    Task<int> ObtenerStockTotalAsync(int idProducto);

    /// <summary>Lista los productos cuyo stock total es menor o igual al stock mínimo.</summary>
    Task<IReadOnlyList<Producto>> ObtenerStockCriticoAsync();
}
