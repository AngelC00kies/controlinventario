using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>
/// Repositorio de stock por almacén. Las lecturas de este repositorio son
/// rastreadas (tracking) porque los servicios modifican las cantidades.
/// </summary>
public class RepositorioStockPorAlmacen : RepositorioBase<StockPorAlmacen>, IRepositorioStockPorAlmacen
{
    public RepositorioStockPorAlmacen(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public async Task<StockPorAlmacen?> ObtenerAsync(int idProducto, int idAlmacen) =>
        await Conjunto
            .Include(s => s.Almacen)
            .FirstOrDefaultAsync(s => s.IdProducto == idProducto && s.IdAlmacen == idAlmacen);

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockPorAlmacen>> ListarPorAlmacenAsync(int idAlmacen) =>
        await Conjunto
            .AsNoTracking()
            .Include(s => s.Producto)
            .Where(s => s.IdAlmacen == idAlmacen)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockPorAlmacen>> ListarPorProductoAsync(int idProducto) =>
        await Conjunto
            .AsNoTracking()
            .Include(s => s.Almacen)
            .Where(s => s.IdProducto == idProducto)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<int> ObtenerStockTotalAsync(int idProducto) =>
        await Conjunto
            .Where(s => s.IdProducto == idProducto)
            .SumAsync(s => s.Cantidad);
}
