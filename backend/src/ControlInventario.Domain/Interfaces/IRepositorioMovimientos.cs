using ControlInventario.Domain.Entidades;
using ControlInventario.Domain.Enums;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de movimientos de inventario.</summary>
public interface IRepositorioMovimientos : IRepositorio<Movimiento>
{
    /// <summary>
    /// Busca movimientos con sus detalles y usuarios, aplicando filtros opcionales:
    /// producto, fechas, usuario y tipo de movimiento.
    /// </summary>
    Task<IReadOnlyList<Movimiento>> BuscarAsync(int? idProducto, DateTime? fechaDesde, DateTime? fechaHasta, int? idUsuario, TipoMovimiento? tipo);

    /// <summary>Obtiene un movimiento por id con sus detalles, usuario, almacén y proveedor.</summary>
    Task<Movimiento?> ObtenerConDetallesAsync(int id);

    /// <summary>Últimos movimientos registrados (para el dashboard).</summary>
    Task<IReadOnlyList<Movimiento>> ObtenerUltimosAsync(int cantidad);
}
