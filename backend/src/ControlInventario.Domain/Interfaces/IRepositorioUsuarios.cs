using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de usuarios (autenticación y gestión).</summary>
public interface IRepositorioUsuarios : IRepositorio<Usuario>
{
    /// <summary>Obtiene un usuario por su nombre de usuario, con rol incluido.</summary>
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);

    /// <summary>Obtiene un usuario por correo, con rol incluido.</summary>
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);

    /// <summary>Indica si ya existe un nombre de usuario o correo registrado.</summary>
    Task<bool> ExisteUsuarioAsync(string nombreUsuario, string correo, int? idUsuarioExcluir = null);

    /// <summary>Lista usuarios incluyendo el nombre del rol.</summary>
    Task<IReadOnlyList<Usuario>> ListarConRolesAsync();
}
