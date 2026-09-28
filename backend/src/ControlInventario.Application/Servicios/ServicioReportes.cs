using ControlInventario.Application.Comun;
using ControlInventario.Application.DTOs;
using ControlInventario.Application.Interfaces;
using ControlInventario.Domain.Interfaces;

namespace ControlInventario.Application.Servicios;

/// <summary>
/// Servicio de reportes: stock actual, movimientos por producto/fecha/usuario
/// y productos con stock crítico.
/// </summary>
public class ServicioReportes : IServicioReportes
{
    private readonly IRepositorioProductos _productos;
    private readonly IRepositorioMovimientos _movimientos;

    public ServicioReportes(IRepositorioProductos productos, IRepositorioMovimientos movimientos)
    {
        _productos = productos;
        _movimientos = movimientos;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockActualDTO>> StockActualAsync(int? idCategoria, bool soloCriticos)
    {
        var productos = await _productos.BuscarAsync(busqueda: null, idCategoria, idProveedor: null, soloActivos: true);

        var filas = productos
            .Select(p =>
            {
                var stock = Mapeador.StockTotal(p);
                return new StockActualDTO
                {
                    IdProducto = p.IdProducto,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria?.Nombre ?? string.Empty,
                    Proveedor = p.Proveedor?.Nombre ?? string.Empty,
                    UnidadMedida = p.UnidadMedida,
                    StockMinimo = p.StockMinimo,
                    StockActual = stock,
                    PrecioCompra = p.PrecioCompra,
                    PrecioVenta = p.PrecioVenta,
                    ValorInventario = stock * p.PrecioCompra,
                    Estado = stock <= p.StockMinimo ? "Crítico" : "Normal"
                };
            })
            .Where(f => !soloCriticos || f.Estado == "Crítico")
            .OrderBy(f => f.Estado == "Crítico" ? 0 : 1)
            .ThenBy(f => f.Nombre)
            .ToList();

        return filas;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MovimientoReporteDTO>> MovimientosAsync(FiltroMovimientoDTO filtros)
    {
        var movimientos = await _movimientos.BuscarAsync(
            filtros.IdProducto, filtros.FechaDesde, filtros.FechaHasta, filtros.IdUsuario, filtros.Tipo);

        var filas = new List<MovimientoReporteDTO>();

        foreach (var movimiento in movimientos)
        {
            foreach (var detalle in movimiento.Detalles)
            {
                // Al filtrar por producto solo se emiten sus líneas.
                if (filtros.IdProducto.HasValue && detalle.IdProducto != filtros.IdProducto.Value)
                {
                    continue;
                }

                filas.Add(new MovimientoReporteDTO
                {
                    IdMovimiento = movimiento.IdMovimiento,
                    Fecha = movimiento.Fecha,
                    Tipo = movimiento.Tipo.ToString(),
                    Motivo = movimiento.Motivo,
                    DocumentoReferencia = movimiento.DocumentoReferencia,
                    Usuario = movimiento.Usuario?.NombreCompleto ?? string.Empty,
                    Almacen = movimiento.Almacen?.Nombre ?? string.Empty,
                    IdProducto = detalle.IdProducto,
                    CodigoProducto = detalle.Producto?.Codigo ?? string.Empty,
                    NombreProducto = detalle.Producto?.Nombre ?? string.Empty,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Subtotal = detalle.Cantidad * detalle.PrecioUnitario
                });
            }
        }

        return filas.OrderByDescending(f => f.Fecha).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AlertaStockDTO>> StockCriticoAsync()
    {
        var criticos = await _productos.ObtenerStockCriticoAsync();
        return criticos
            .Select(Mapeador.AAlertaStockDTO)
            .OrderByDescending(a => a.Diferencia)
            .ToList();
    }
}
