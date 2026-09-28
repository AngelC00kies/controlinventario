using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de gestión de almacenes.</summary>
public interface IServicioAlmacenes
{
    Task<IReadOnlyList<AlmacenDTO>> ListarAsync(bool soloActivos = true);

    Task<AlmacenDTO> ObtenerPorIdAsync(int id);

    Task<AlmacenDTO> CrearAsync(AlmacenCreacionDTO dto);

    Task<AlmacenDTO> ActualizarAsync(int id, AlmacenActualizacionDTO dto);

    Task EliminarAsync(int id);
}
