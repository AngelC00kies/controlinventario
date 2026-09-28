namespace ControlInventario.Domain.Interfaces;

/// <summary>
/// Contrato genérico de repositorio (DIP: las capas superiores dependen
/// de esta abstracción y no de la implementación concreta en Infrastructure).
/// </summary>
/// <typeparam name="TEntidad">Entidad de dominio con clave primaria int.</typeparam>
public interface IRepositorio<TEntidad> where TEntidad : class
{
    /// <summary>Obtiene una entidad por su identificador, o null si no existe.</summary>
    Task<TEntidad?> ObtenerPorIdAsync(int id);

    /// <summary>Listado completo de entidades.</summary>
    Task<IReadOnlyList<TEntidad>> ListarAsync();

    /// <summary>Agrega una nueva entidad.</summary>
    Task AgregarAsync(TEntidad entidad);

    /// <summary>Marca una entidad como modificada (se persiste al guardar cambios).</summary>
    void Actualizar(TEntidad entidad);

    /// <summary>Elimina una entidad.</summary>
    void Eliminar(TEntidad entidad);
}
