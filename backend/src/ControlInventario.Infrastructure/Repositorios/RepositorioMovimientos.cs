using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;
using ControlInventario.Domain.Interfaces;
using ControlInventario.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace ControlInventario.Infrastructure.Repositorios;

/// <summary>Repositorio de movimientos con sus detalles y filtros de consulta.</summary>
public class RepositorioMovimientos : RepositorioBase<Movimiento>, IRepositorioMovimientos
{
    public RepositorioMovimientos(ContextoControlInventario contexto) : base(contexto)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Movimiento>> BuscarAsync(int? idProducto, DateTime? fechaDesde, DateTime? fechaHasta, int? idUsuario, TipoMovimiento? tipo)
    {
        var consulta = Conjunto
            .AsNoTracking()
            .Include(m => m.Detalles).ThenInclude(d => d.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.Almacen)
            .Include(m => m.Proveedor)
            .AsQueryable();

        if (idProducto.HasValue)
        {
            consulta = consulta.Where(m => m.Detalles.Any(d => d.IdProducto == idProducto.Value));
        }

        if (fechaDesde.HasValue)
        {
            var desde = fechaDesde.Value;
            consulta = consulta.Where(m => m.Fecha >= desde);
        }

        if (fechaHasta.HasValue)
        {
            // Se incluye todo el día de la fecha final.
            var hasta = fechaHasta.Value.Date.AddDays(1);
            consulta = consulta.Where(m => m.Fecha < hasta);
        }

        if (idUsuario.HasValue)
        {
            consulta = consulta.Where(m => m.IdUsuario == idUsuario.Value);
        }

        if (tipo.HasValue)
        {
            consulta = consulta.Where(m => m.Tipo == tipo.Value);
        }

        return await consulta.OrderByDescending(m => m.Fecha).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Movimiento?> ObtenerConDetallesAsync(int id) =>
        await Conjunto
            .AsNoTracking()
            .Include(m => m.Detalles).ThenInclude(d => d.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.Almacen)
            .Include(m => m.Proveedor)
            .FirstOrDefaultAsync(m => m.IdMovimiento == id);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Movimiento>> ObtenerUltimosAsync(int cantidad) =>
        await Conjunto
            .AsNoTracking()
            .Include(m => m.Detalles).ThenInclude(d => d.Producto)
            .Include(m => m.Usuario)
            .Include(m => m.Almacen)
            .Include(m => m.Proveedor)
            .OrderByDescending(m => m.Fecha)
            .Take(cantidad)
            .ToListAsync();
}
