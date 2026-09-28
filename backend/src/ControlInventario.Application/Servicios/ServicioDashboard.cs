using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Domain.Interfaces;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio del dashboard: consolida los indicadores principales del sistema.
/// </summary>
public class ServicioDashboard : IServicioDashboard
{
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioAlmacenes _almacenes;
    private readonly IRepositorioProveedores _proveedores;
    private readonly IRepositorioMovimientos _movimientos;

    public ServicioDashboard(
        IRepositorioProductos productos,
        IRepositorioAlmacenes almacenes,
        IRepositorioProveedores proveedores,
        IRepositorioMovimientos movimientos)
    {
        _productos = productos;
        _almacenes = almacenes;
        _proveedores = proveedores;
        _movimientos = movimientos;
    }

    /// <inheritdoc />
    public async Task<ResumenDashboardDTO> ResumenAsync()
    {
        var productos = await _productos.BuscarAsync(busqueda: null, idCategoria: null, idProveedor: null, soloActivos: true);
        var almacenes = await _almacenes.ListarAsync();
        var proveedores = await _proveedores.ListarAsync();

        // Movimientos del mes en curso.
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var movimientosMes = await _movimientos.BuscarAsync(
            idProducto: null, fechaDesde: inicioMes, fechaHasta: null, idUsuario: null, tipo: null);

        var productosCriticos = productos.Where(p => Mapeador.StockTotal(p) <= p.StockMinimo).ToList();

        return new ResumenDashboardDTO
        {
            TotalProductos = productos.Count(p => p.Activo),
            TotalAlmacenes = almacenes.Count(a => a.Activo),
            TotalProveedores = proveedores.Count(p => p.Activo),
            MovimientosMes = movimientosMes.Count,
            ValorInventario = productos.Sum(p => Mapeador.StockTotal(p) * p.PrecioCompra),
            ProductosStockCritico = productosCriticos.Count,
            // Máximo 5 alertas para no saturar la portada.
            Alertas = productosCriticos
                .Select(Mapeador.AAlertaStockDTO)
                .OrderByDescending(a => a.Diferencia)
                .Take(5)
                .ToList(),
            UltimosMovimientos = (await _movimientos.ObtenerUltimosAsync(5))
                .Select(Mapeador.AMovimientoDTO)
                .ToList()
        };
    }
}
