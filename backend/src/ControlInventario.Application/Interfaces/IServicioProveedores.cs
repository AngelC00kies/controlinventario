using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de gestión de proveedores.</summary>
public interface IServicioProveedores
{
    Task<IReadOnlyList<ProveedorDTO>> ListarAsync(bool soloActivos = true);

    Task<ProveedorDTO> ObtenerPorIdAsync(int id);

    Task<ProveedorDTO> CrearAsync(ProveedorCreacionDTO dto);

    Task<ProveedorDTO> ActualizarAsync(int id, ProveedorActualizacionDTO dto);

    Task EliminarAsync(int id);
}
