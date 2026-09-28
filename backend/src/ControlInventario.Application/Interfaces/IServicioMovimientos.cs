using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de movimientos de inventario (entradas y salidas).</summary>
public interface IServicioMovimientos
{
    /// <summary>Busca movimientos con filtros opcionales (producto, fechas, usuario, tipo).</summary>
    Task<IReadOnlyList<MovimientoDTO>> BuscarAsync(FiltroMovimientoDTO filtros);

    /// <summary>Obtiene un movimiento por id con sus detalles.</summary>
    Task<MovimientoDTO> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Registra un movimiento. Actualiza el stock automáticamente en una transacción:
    /// las entradas suman y las salidas restan (validando stock suficiente).
    /// </summary>
    Task<MovimientoDTO> CrearAsync(MovimientoCreacionDTO dto, int idUsuario);
}
