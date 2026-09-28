using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de productos con las consultas específicas del inventario.</summary>
public class RepositorioProductos : RepositorioBase<Producto>, IRepositorioProductos
{
    public RepositorioProductos(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public Task<bool> ExisteCodigoAsync(string codigo, int? idProductoExcluir = null) =>
        Conjunto.AnyAsync(p =>
            p.Codigo == codigo &&
            (!idProductoExcluir.HasValue || p.IdProducto != idProductoExcluir.Value));

    /// <inheritdoc />
    public async Task<IReadOnlyList<Producto>> BuscarAsync(string? busqueda, int? idCategoria, int? idProveedor, bool soloActivos = true)
    {
        var consulta = Conjunto
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Include(p => p.StockPorAlmacen)
            .AsQueryable();

        if (soloActivos)
        {
            consulta = consulta.Where(p => p.Activo);
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            consulta = consulta.Where(p =>
                p.Codigo.Contains(termino) ||
                p.Nombre.Contains(termino) ||
                (p.Descripcion != null && p.Descripcion.Contains(termino)));
        }

        if (idCategoria.HasValue)
        {
            consulta = consulta.Where(p => p.IdCategoria == idCategoria.Value);
        }

        if (idProveedor.HasValue)
        {
            consulta = consulta.Where(p => p.IdProveedor == idProveedor.Value);
        }

        return await consulta.OrderBy(p => p.Nombre).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Producto?> ObtenerCompletoAsync(int id) =>
        await Conjunto
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Include(p => p.StockPorAlmacen).ThenInclude(s => s.Almacen)
            .FirstOrDefaultAsync(p => p.IdProducto == id);

    /// <inheritdoc />
    public async Task<int> ObtenerStockTotalAsync(int idProducto) =>
        await Conjunto
            .Where(p => p.IdProducto == idProducto)
            .SelectMany(p => p.StockPorAlmacen)
            .SumAsync(s => s.Cantidad);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Producto>> ObtenerStockCriticoAsync() =>
        await Conjunto
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.StockPorAlmacen)
            .Where(p => p.Activo &&
                        (p.StockPorAlmacen.Sum(s => (int?)s.Cantidad) ?? 0) <= p.StockMinimo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
}
