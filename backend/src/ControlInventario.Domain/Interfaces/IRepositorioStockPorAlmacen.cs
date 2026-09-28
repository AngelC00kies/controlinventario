using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de stock por almacén.</summary>
public interface IRepositorioStockPorAlmacen : IRepositorio<StockPorAlmacen>
{
    /// <summary>Obtiene el registro de stock de un producto en un almacén (o null).</summary>
    Task<StockPorAlmacen?> ObtenerAsync(int idProducto, int idAlmacen);

    /// <summary>Lista el stock de todos los productos de un almacén.</summary>
    Task<IReadOnlyList<StockPorAlmacen>> ListarPorAlmacenAsync(int idAlmacen);

    /// <summary>Lista el stock de un producto en todos los almacenes.</summary>
    Task<IReadOnlyList<StockPorAlmacen>> ListarPorProductoAsync(int idProducto);

    /// <summary>Suma total de un producto en todos los almacenes.</summary>
    Task<int> ObtenerStockTotalAsync(int idProducto);
}
