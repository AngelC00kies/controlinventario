using ControlInventario.Domain.Entidades;

namespace ControlInventario.Domain.Interfaces;

/// <summary>Contrato del repositorio de roles.</summary>
public interface IRepositorioRoles : IRepositorio<Rol>
{
    /// <summary>Obtiene un rol por su nombre (ej. "Administrador").</summary>
    Task<Rol?> ObtenerPorNombreAsync(string nombre);
}
