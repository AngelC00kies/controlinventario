using ControlInventario.Application.DTOs;

namespace ControlInventario.Application.Interfaces;

/// <summary>Servicio de gestión de usuarios y roles.</summary>
public interface IServicioUsuarios
{
    Task<IReadOnlyList<UsuarioDTO>> ListarAsync();

    Task<IReadOnlyList<RolDTO>> ObtenerRolesAsync();

    Task<UsuarioDTO> ObtenerPorIdAsync(int id);

    Task<UsuarioDTO> CrearAsync(UsuarioCreacionDTO dto);

    Task<UsuarioDTO> ActualizarAsync(int id, UsuarioActualizacionDTO dto);

    Task EliminarAsync(int id);
}
